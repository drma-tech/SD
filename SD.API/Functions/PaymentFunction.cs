using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.API.Core.Models;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Subscription;
using Stripe.Checkout;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SD.API.Functions;

public class PaymentFunction(CosmosRepository repo, IHttpClientFactory factory)
{
    [Function("PaymentConfigurations")]
    public static PaymentConfigurations PaymentConfigurations(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/payment/configurations")] HttpRequestData req)
    {
        try
        {
            var valid = Enum.TryParse(req.GetQueryParameters()["provider"], out PaymentProvider provider);
            if (!valid) throw new UnhandledException("invalid provider");

            if (provider == PaymentProvider.Apple)
            {
                return new PaymentConfigurations
                {
                    PricePremiumWeek = ApiStartup.Configurations.Apple?.Premium?.PriceWeek,
                    PricePremiumMonth = ApiStartup.Configurations.Apple?.Premium?.PriceMonth,
                    PricePremiumYear = ApiStartup.Configurations.Apple?.Premium?.PriceYear
                };
            }
            else if (provider == PaymentProvider.Stripe)
            {
                return new PaymentConfigurations
                {
                    PricePremiumWeek = ApiStartup.Configurations.Stripe?.Premium?.PriceWeek,
                    PricePremiumMonth = ApiStartup.Configurations.Stripe?.Premium?.PriceMonth,
                    PricePremiumYear = ApiStartup.Configurations.Stripe?.Premium?.PriceYear
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

    [Function("PostAppleVerify")]
    public async Task PostAppleVerify(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "apple/verify")] HttpRequestData req, CancellationToken cancellationToken)
    {
        AuthPrincipal? client = null;
        try
        {
            var userId = await req.GetUserIdAsync(cancellationToken);
            var ip = req.GetUserIP(true);

            client = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken) ?? throw new UnhandledException("principal null");

            var raw = await req.ReadAsStringAsync();
            var receipt = JsonSerializer.Deserialize<string>(raw ?? throw new UnhandledException("body not present"));

            var bundleId = ApiStartup.Configurations.Apple?.BundleId;

            var result = await VerifyReceipt(ApiStartup.Configurations.Apple?.Endpoint, receipt, cancellationToken) ?? throw new UnhandledException("AppleResponseReceipt null");
            if (result.status == 21007)
            {
                //when tested with TestFlight
                result = await VerifyReceipt("https://sandbox.itunes.apple.com/", receipt, cancellationToken) ?? throw new UnhandledException("AppleResponseReceipt null");
            }

            if (result.status != 0) throw new UnhandledException($"invalid status: {result.status}");
            if (result.receipt!.bundle_id != bundleId) throw new UnhandledException("invalid receipt");

            var purchase = result.latest_receipt_info[result.latest_receipt_info.Count - 1];

            var sub = new AuthSubscription
            {
                Provider = PaymentProvider.Apple,
                Product = AccountProduct.Premium,
                Cycle = purchase.product_id!.Contains("yearly") ? AccountCycle.Yearly : AccountCycle.Monthly,
                SessionId = receipt //save receipt before cause it may fail
            };

            sub.SubscriptionId = purchase.original_transaction_id;
            sub.ExpiresDate = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(purchase.expires_date_ms ?? "0", CultureInfo.InvariantCulture));

            client.AddSubscription(sub);

            //https://developer.apple.com/documentation/appstorereceipts/status
            client.Events.Add(new Event("Apple", $"Subscription created with status = {result.status} and id = {purchase.original_transaction_id}", ip));
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

    private async Task<AppleResponseReceipt?> VerifyReceipt(string? endpoint, string? receipt, CancellationToken cancellationToken)
    {
        var http = factory.CreateClient("apple");
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}verifyReceipt");
        request.Content = new StringContent($$"""{"receipt-data":"{{receipt}}","password":"{{ApiStartup.Configurations.Apple?.SharedSecret}}","exclude-old-transactions":true}""", Encoding.UTF8, "application/json");
        var response = await http.SendAsync(request, cancellationToken);
        return await response.Content.ReadFromJsonAsync<AppleResponseReceipt>(cancellationToken);
    }

    [Function("PostAppleWebhook")]
    public async Task PostAppleWebhook(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/apple/webhook")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var ip = req.GetUserIP(true);

            var body = await req.ReadFromJsonAsync<Dictionary<string, string>>(cancellationToken) ?? throw new UnhandledException("body null");

            if (!body.TryGetValue("signedPayload", out var signedPayload)) throw new UnhandledException("signedPayload null");

            var notification = AppleJwtDecoder.DecodeServerNotification(signedPayload, ApiStartup.Configurations.Apple!);

            var info = notification.Data;

            var transaction = AppleJwtDecoder.DecodeTransaction(info.SignedTransactionInfo);

            var originalTransactionId = transaction.OriginalTransactionId;

            var results = await repo.Query<AuthPrincipal>(x => x.Subscriptions.Any(p => p.SubscriptionId == originalTransactionId), DocumentType.Principal, cancellationToken);

            var client = results.LastOrDefault();

            if (client == null)
            {
                req.LogError(new UnhandledException($"client null - originalTransactionId:{originalTransactionId}"));
                return;
            }

            var sub = client.GetSubscription(originalTransactionId, PaymentProvider.Apple);

            if (notification.NotificationType == "REFUND" || notification.NotificationType == "REVOKE")
            {
                sub.ExpiresDate = DateTimeOffset.UtcNow; //disable immediately
            }
            else
            {
                var newExpires = DateTimeOffset.FromUnixTimeMilliseconds(transaction.ExpiresDate);
                if (sub.ExpiresDate == null || newExpires > sub.ExpiresDate)
                {
                    sub.ExpiresDate = newExpires;
                }
            }

            var product = transaction.ProductId ?? throw new UnhandledException("product not available");
            sub.Cycle = product.Contains("yearly") ? AccountCycle.Yearly : AccountCycle.Monthly;

            client.UpdateSubscription(sub);

            client.Events.Add(new Event("Apple (Webhooks)", $"SubscriptionId = {originalTransactionId}, Cycle = {sub.Cycle}, Type = {notification.NotificationType}, Subtype = {notification.Subtype}, expiresDate = {sub.ExpiresDate}", ip));

            await repo.UpsertItemAsync(client, cancellationToken);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("StripeCreateCustomer")]
    public async Task<AuthPrincipal> StripeCreateCustomer(
   [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "stripe/customer")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(cancellationToken);
            var principal = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken) ?? throw new UnhandledException("principal null");

            var customer = await new Stripe.CustomerService().CreateAsync(new Stripe.CustomerCreateOptions
            {
                Name = principal.DisplayName,
                Email = principal.Email,
            }, cancellationToken: cancellationToken);

            principal.StripeCustomerId = customer.Id;

            var ip = req.GetUserIP(true);
            principal.Events.Add(new Event("Stripe", $"User registered with id:{customer.Id}", ip));

            return await repo.UpsertItemAsync(principal, cancellationToken);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("CreateCheckoutSession")]
    public async Task<string> CreateCheckoutSession(
      [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "stripe/create-checkout-session/{priceId}")] HttpRequestData req, string priceId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(cancellationToken) ?? throw new NotificationException("user not available");
            var ip = req.GetUserIP(true);
            var url = req.GetQueryParameters()["url"];

            var principal = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken) ?? throw new UnhandledException("principal null");

            if (principal.StripeCustomerId.Empty()) throw new NotificationException("Stripe customer not available");

            var options = new SessionCreateOptions
            {
                Customer = principal.StripeCustomerId,

                LineItems = [new() { Price = priceId, Quantity = 1, },],
                Mode = "subscription",
                SuccessUrl = url + "?stripe_session_id={CHECKOUT_SESSION_ID}",
            };

            options.AddExtraParam("managed_payments[enabled]", true);

            var service = new SessionService();
            Session session = await service.CreateAsync(options, cancellationToken: cancellationToken);

            AccountCycle? cycle = null;

            if (priceId == ApiStartup.Configurations.Stripe!.Premium!.PriceWeek)
                cycle = AccountCycle.Weekly;
            else if (priceId == ApiStartup.Configurations.Stripe.Premium.PriceMonth)
                cycle = AccountCycle.Monthly;
            else if (priceId == ApiStartup.Configurations.Stripe.Premium.PriceYear)
                cycle = AccountCycle.Yearly;

            var sub = new AuthSubscription()
            {
                Provider = PaymentProvider.Stripe,
                Product = AccountProduct.Premium,
                Cycle = cycle,
                SessionId = session.Id
            };

            principal.AddSubscription(sub);

            principal.Events.Add(new Event("Stripe", $"Session created with cycle = {cycle} and SessionId = {session.Id}", ip));

            await repo.UpsertItemAsync(principal, cancellationToken);

            return session.Url;
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("PostStripeWebhook")]
    public async Task PostStripeWebhook(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/stripe/webhook")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var json = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);

            req.Headers.TryGetValues("Stripe-Signature", out var Signature);
            if (string.IsNullOrEmpty(Signature?.First())) throw new UnhandledException("Stripe signature missing");
            var stripeEvent = Stripe.EventUtility.ConstructEvent(json, Signature?.First(), ApiStartup.Configurations.Stripe?.SigningSecret ?? throw new UnhandledException("Stripe SigningSecret not configured"), throwOnApiVersionMismatch: false);

            if (stripeEvent.Type.StartsWith("customer.subscription")) //created, updated, deleted, paused, resumed, trial_will_end, pending_update_applied, pending_update_expired
            {
                if (stripeEvent.Data.Object is not Stripe.Subscription subscription || subscription.Id.Empty()) throw new UnhandledException("stripe subscription not available");

                var results = await repo.Query<AuthPrincipal>(p => p.StripeCustomerId == subscription.CustomerId, DocumentType.Principal, cancellationToken) ?? throw new UnhandledException("AuthPrincipal null");
                var principal = results.SingleOrDefault();

                if (principal == null)
                {
                    req.LogError(new UnhandledException($"principal null - subscriptionId:{subscription.Id}"));
                    return;
                }

                var sub = principal.GetSubscription(subscription.Id, PaymentProvider.Stripe);

                sub.Active = subscription.Status is "active" or "trialing";

                sub.Cycle = Enum.Parse<AccountCycle>(subscription.Items.First().Price.Metadata["cycle"]); //if cycle changes, update it

                if (subscription.CancelAt.HasValue)
                {
                    sub.ExpiresDate = subscription.CancelAt.Value;
                }

                principal.UpdateSubscription(sub);

                var ip = req.GetUserIP(true);
                var type = stripeEvent.Type.Split(".")[2];
                principal.Events.Add(new Event("Stripe (Webhooks)", $"Type = {type}, Status = {subscription.Status}, Cycle = {sub.Cycle} for SubscriptionId = {subscription.Id}", ip));

                await repo.UpsertItemAsync(principal, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }
}