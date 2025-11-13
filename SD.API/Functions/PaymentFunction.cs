using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.API.Core.Models;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Subscription;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
            req.ProcessException(ex);
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
            req.ProcessException(ex);
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
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("PostAppleVerify")]
    public async Task PostAppleVerify(
      [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "apple/verify")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);
            var client = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken) ?? throw new UnhandledException("principal null");

            var receipt = await req.ReadAsStringAsync();
            var endpoint = ApiStartup.Configurations.Apple?.Endpoint;
            var bundleId = ApiStartup.Configurations.Apple?.BundleId;

            var http = factory.CreateClient("apple");
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}verifyReceipt");
            request.Content = JsonContent.Create(new AppleRequestModel { ReceiptData = receipt, Password = ApiStartup.Configurations.Apple?.SharedSecret, ExcludeOldTransactions = true });
            var response = await http.SendAsync(request, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<AppleResponseReceipt>(cancellationToken);

            if (result == null) throw new UnhandledException("AppleResponseReceipt null");
            if (result.status != 0) throw new UnhandledException($"invalid status: {result.status}");
            if (result.receipt!.bundle_id != bundleId) throw new UnhandledException("invalid receipt");

            var purchase = result.latest_receipt_info!.Single();

            client.Subscription ??= new AuthSubscription();

            client.Subscription.SubscriptionId = purchase.original_transaction_id;
            client.Subscription.LatestReceipt = receipt;
            client.Subscription.ExpiresDate = purchase.expires_date?.ParseAppleDate();

            client.Subscription.Provider = PaymentProvider.Apple;
            client.Subscription.Product = purchase.product_id!.Contains("premium") ? AccountProduct.Premium : AccountProduct.Standard;
            client.Subscription.Cycle = purchase.product_id!.Contains("yearly") ? AccountCycle.Yearly : AccountCycle.Monthly;

            //https://developer.apple.com/documentation/appstorereceipts/status
            client.Events = client.Events.Union([new Event { Description = $"subscription = {receipt}, status = {result.status}" }]).ToArray();

            await repo.UpsertItemAsync(client, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("PostAppleSubscription")]
    public async Task PostAppleSubscription(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/apple/subscription")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            //var validSignature = await req.ValidPaddleSignature(ApiStartup.Configurations.Paddle?.Signature, cancellationToken);

            //if (!validSignature) throw new UnhandledException("wrong paddle signature");

            //var body = await req.GetPublicBody<RootEvent>(cancellationToken) ?? throw new UnhandledException("body null");
            //if (body.data == null) throw new UnhandledException("body.data null");

            //await Task.Delay(1000, cancellationToken); //wait for user be updated in cosmos

            //var result = await repo.Query<AuthPrincipal>(x => x.AuthPaddle != null && x.AuthPaddle.CustomerId == body.data.customer_id, DocumentType.Principal, cancellationToken) ??
            //    throw new UnhandledException("AuthPrincipal null");
            //var client = result.LastOrDefault() ?? throw new UnhandledException($"client null - customer_id:{body.data.customer_id}");
            //if (client.AuthPaddle == null) throw new UnhandledException("client.AuthPaddle null");

            //client.AuthPaddle.SubscriptionId = body.data.id;
            //client.AuthPaddle.IsPaidUser = body.data.status is "active" or "trialing";

            //client.Events = client.Events.Union([new Event { Description = $"subscription = {body.data.status}" }]).ToArray();

            //await repo.UpsertItemAsync(client, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
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
            req.ProcessException(ex);
            throw;
        }
    }
}