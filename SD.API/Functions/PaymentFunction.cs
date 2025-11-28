using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.API.Core.Models;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Subscription;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SD.API.Functions;

public class PaymentFunction(CosmosRepository repo, IHttpClientFactory factory)
{
    [Function("GetSubscription")]
    public async Task<PaddleRootSubscription?> GetSubscription(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/paddle/subscription")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var id = req.GetQueryParameters()["id"];
            if (id.Empty()) throw new UnhandledException("id null");

            var endpoint = ApiStartup.Configurations.Paddle?.Endpoint;
            var key = ApiStartup.Configurations.Paddle?.Key;

            var http = factory.CreateClient("paddle");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}subscriptions/{id}");

            var response = await http.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode) throw new UnhandledException(response.ReasonPhrase);

            return await response.Content.ReadFromJsonAsync<PaddleRootSubscription>(cancellationToken);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("CreateCustomer")]
    public async Task CreateCustomer(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "paddle/customer")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);
            var principal = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken) ?? throw new UnhandledException("principal null");

            var endpoint = ApiStartup.Configurations.Paddle?.Endpoint;
            var key = ApiStartup.Configurations.Paddle?.Key;

            var http = factory.CreateClient("paddle");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}customers");

            request.Content = JsonContent.Create(new { email = principal.Email, name = principal.DisplayName });

            var response = await http.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PaddleCustomerResponse>(cancellationToken);

                principal.Subscription = new AuthSubscription
                {
                    Provider = PaymentProvider.Paddle,
                    CustomerId = result?.data?.id
                };

                await repo.UpsertItemAsync(principal, cancellationToken);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                using var requestOld = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}customers?email={principal.Email}");
                var responseOld = await http.SendAsync(requestOld, cancellationToken);

                if (!responseOld.IsSuccessStatusCode) throw new UnhandledException(response.ReasonPhrase);

                var result = await responseOld.Content.ReadFromJsonAsync<PaddleCustomerResponseArray>(cancellationToken);

                principal.Subscription = new AuthSubscription
                {
                    Provider = PaymentProvider.Paddle,
                    CustomerId = result?.data.Single().id
                };

                await repo.UpsertItemAsync(principal, cancellationToken);
            }
            else
            {
                throw new UnhandledException(response.ReasonPhrase);
            }
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("PostPaddleSubscription")]
    public async Task PostPaddleSubscription(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/paddle/subscription")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var validSignature = await req.ValidPaddleSignature(ApiStartup.Configurations.Paddle?.Signature, cancellationToken);

            if (!validSignature) throw new UnhandledException("wrong paddle signature");

            var body = await req.GetPublicBody<RootEvent>(cancellationToken) ?? throw new UnhandledException("body null");
            if (body.data == null) throw new UnhandledException("body.data null");

            var result = await repo.Query<AuthPrincipal>(x => x.Subscription != null && x.Subscription.CustomerId == body.data.customer_id, DocumentType.Principal, cancellationToken) ??
                throw new UnhandledException("AuthPrincipal null");
            var client = result.LastOrDefault() ?? throw new UnhandledException($"client null - customer_id:{body.data.customer_id}");
            if (client.Subscription == null) throw new UnhandledException("client.Subscription null");

            client.Subscription.SubscriptionId = body.data.id;
            client.Subscription.Active = body.data.status is "active" or "trialing";

            client.Subscription.Provider = PaymentProvider.Paddle;
            client.Subscription.Product = body.data.items[0].price?.custom_data?.ProductEnum;
            client.Subscription.Cycle = body.data.items[0].price?.custom_data?.CycleEnum;

            client.Events = client.Events.Union([new Event { Description = $"subscription = {body.data.id}, status = {body.data.status}" }]).ToArray();

            await repo.UpsertItemAsync(client, cancellationToken);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("PostAppleVerify")]
    public async Task PostAppleVerify(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "apple/verify")] HttpRequestData req, CancellationToken cancellationToken)
    {
        AuthPrincipal? client = null;
        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);
            client = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken) ?? throw new UnhandledException("principal null");

            var raw = await req.ReadAsStringAsync();
            var receipt = JsonSerializer.Deserialize<string>(raw ?? throw new NotificationException("body not present"));
            var endpoint = ApiStartup.Configurations.Apple?.Endpoint;
            var bundleId = ApiStartup.Configurations.Apple?.BundleId;

            client.Subscription ??= new AuthSubscription();
            client.Subscription.LatestReceipt = receipt; //save receipt before cause it may fail

            var http = factory.CreateClient("apple");
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}verifyReceipt");
            request.Content = new StringContent($$"""{"receipt-data":"{{receipt}}","password":"{{ApiStartup.Configurations.Apple?.SharedSecret}}","exclude-old-transactions":true}""", Encoding.UTF8, "application/json");
            var response = await http.SendAsync(request, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<AppleResponseReceipt>(cancellationToken);

            if (result == null) throw new UnhandledException("AppleResponseReceipt null");
            if (result.status != 0) throw new UnhandledException($"invalid status: {result.status}");
            if (result.receipt!.bundle_id != bundleId) throw new UnhandledException("invalid receipt");

            var purchase = result.latest_receipt_info[result.latest_receipt_info.Count - 1];

            client.Subscription.SubscriptionId = purchase.original_transaction_id;
            client.Subscription.ExpiresDate = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(purchase.expires_date_ms ?? "0", CultureInfo.InvariantCulture));

            client.Subscription.Provider = PaymentProvider.Apple;
            client.Subscription.Product = purchase.product_id!.Contains("premium") ? AccountProduct.Premium : AccountProduct.Standard;
            client.Subscription.Cycle = purchase.product_id!.Contains("yearly") ? AccountCycle.Yearly : AccountCycle.Monthly;

            //https://developer.apple.com/documentation/appstorereceipts/status
            client.Events = client.Events.Union([new Event { Description = $"apple verify || subscription = {purchase.original_transaction_id}, status = {result.status}" }]).ToArray();
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
        finally
        {
            if (client != null) await repo.UpsertItemAsync(client, cancellationToken);
        }
    }

    [Function("PostAppleSubscription")]
    public async Task PostAppleSubscription(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/apple/subscription")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var body = await req.ReadFromJsonAsync<Dictionary<string, string>>(cancellationToken) ?? throw new UnhandledException("body null");

            if (!body.TryGetValue("signedPayload", out var signedPayload)) throw new UnhandledException("signedPayload null");

            var notification = AppleJwtDecoder.DecodeServerNotification(signedPayload, ApiStartup.Configurations.Apple!);

            var info = notification.Data;

            var transaction = AppleJwtDecoder.DecodeTransaction(info.SignedTransactionInfo);

            var originalTransactionId = transaction.OriginalTransactionId;

            var results = await repo.Query<AuthPrincipal>(x => x.Subscription != null && x.Subscription.SubscriptionId == originalTransactionId, DocumentType.Principal, cancellationToken);

            var client = results.LastOrDefault() ?? throw new UnhandledException($"client null - originalTransactionId:{originalTransactionId}");

            if (client.Subscription == null) throw new UnhandledException("client.Subscription null");

            var newExpires = DateTimeOffset.FromUnixTimeMilliseconds(transaction.ExpiresDate);
            if (client.Subscription.ExpiresDate == null || newExpires > client.Subscription.ExpiresDate)
            {
                client.Subscription.ExpiresDate = newExpires;
            }
            client.Subscription.Product = transaction.ProductId!.Contains("premium") ? AccountProduct.Premium : AccountProduct.Standard;
            client.Subscription.Cycle = transaction.ProductId!.Contains("yearly") ? AccountCycle.Yearly : AccountCycle.Monthly;

            client.Events = client.Events.Union([new Event {
                Description = $"apple subscription || subscription = {originalTransactionId}, product = {client.Subscription.Product}, Cycle = {client.Subscription.Cycle}, Type = {notification.NotificationType}, Subtype = {notification.Subtype}, expiresDate = {newExpires}"
            }]).ToArray();

            await repo.UpsertItemAsync(client, cancellationToken);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("PaymentConfigurations")]
    public static PaymentConfigurations PaymentConfigurations(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/payment/configurations")] HttpRequestData req)
    {
        try
        {
            var valid = Enum.TryParse(req.GetQueryParameters()["provider"], out PaymentProvider provider);
            if (!valid) throw new UnhandledException("invalid provider");

            if (provider == PaymentProvider.Paddle)
            {
                return new PaymentConfigurations
                {
                    CustomerPortalEndpoint = ApiStartup.Configurations.Paddle?.CustomerPortalEndpoint,
                    Token = ApiStartup.Configurations.Paddle?.Token,
                    PriceStandardMonth = ApiStartup.Configurations.Paddle?.Standard?.PriceMonth,
                    PriceStandardYear = ApiStartup.Configurations.Paddle?.Standard?.PriceYear,
                    PricePremiumMonth = ApiStartup.Configurations.Paddle?.Premium?.PriceMonth,
                    PricePremiumYear = ApiStartup.Configurations.Paddle?.Premium?.PriceYear
                };
            }
            else if (provider == PaymentProvider.Apple)
            {
                return new PaymentConfigurations
                {
                    PriceStandardMonth = ApiStartup.Configurations.Apple?.Standard?.PriceMonth,
                    PriceStandardYear = ApiStartup.Configurations.Apple?.Standard?.PriceYear,
                    PricePremiumMonth = ApiStartup.Configurations.Apple?.Premium?.PriceMonth,
                    PricePremiumYear = ApiStartup.Configurations.Apple?.Premium?.PriceYear
                };
            }
            else
            {
                throw new UnhandledException("provider not implemented");
            }
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }
}