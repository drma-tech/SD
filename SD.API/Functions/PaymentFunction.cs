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

            var sub = new AuthSubscription
            {
                Provider = PaymentProvider.Apple,
                LatestReceipt = receipt //save receipt before cause it may fail
            };

            var result = await VerifyReceipt(ApiStartup.Configurations.Apple?.Endpoint, receipt, cancellationToken) ?? throw new UnhandledException("AppleResponseReceipt null");
            if (result.status == 21007)
            {
                //when tested with TestFlight
                result = await VerifyReceipt("https://sandbox.itunes.apple.com/", receipt, cancellationToken) ?? throw new UnhandledException("AppleResponseReceipt null");
            }

            if (result.status != 0) throw new UnhandledException($"invalid status: {result.status}");
            if (result.receipt!.bundle_id != bundleId) throw new UnhandledException("invalid receipt");

            var purchase = result.latest_receipt_info[result.latest_receipt_info.Count - 1];

            sub.SubscriptionId = purchase.original_transaction_id;
            sub.ExpiresDate = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(purchase.expires_date_ms ?? "0", CultureInfo.InvariantCulture));

            sub.Product = AccountProduct.Premium;
            sub.Cycle = purchase.product_id!.Contains("yearly") ? AccountCycle.Yearly : AccountCycle.Monthly;

            client.AddSubscription(sub);

            //https://developer.apple.com/documentation/appstorereceipts/status
            client.Events.Add(new Event("Apple", $"New status ({result.status}) for SubscriptionId ({purchase.original_transaction_id})", ip));
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
            sub.Product = AccountProduct.Premium;
            sub.Cycle = product.Contains("yearly") ? AccountCycle.Yearly : AccountCycle.Monthly;

            client.UpdateSubscription(sub);

            client.Events.Add(new Event("Apple (Webhooks)", $"SubscriptionId = {originalTransactionId}, Product = {sub.Product}, Cycle = {sub.Cycle}, Type = {notification.NotificationType}, Subtype = {notification.Subtype}, expiresDate = {sub.ExpiresDate}", ip));

            await repo.UpsertItemAsync(client, cancellationToken);
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
            var userId = await req.GetUserIdAsync(cancellationToken);
            var ip = req.GetUserIP(true);

            var principal = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken) ?? throw new UnhandledException("principal null");

            var domain = "https://localhost:7272";

            var options2 = new SessionCreateOptions
            {
                CustomerEmail = principal.Email,
                ClientReferenceId = userId,
                LineItems = [new() { Price = priceId, Quantity = 1, },],
                Mode = "subscription",
                SuccessUrl = domain + "/?session_id={CHECKOUT_SESSION_ID}",
            };

            options2.AddExtraParam("managed_payments[enabled]", true);

            var service = new SessionService();
            Session session = await service.CreateAsync(options2, cancellationToken: cancellationToken);

            var sub = new AuthSubscription()
            {
                Provider = PaymentProvider.Stripe,
                CustomerId = session.CustomerId,
                LatestReceipt = session.Id
            };

            principal.AddSubscription(sub);

            principal.Events.Add(new Event("Stripe", $"User registration with CustomerId {session.CustomerId} and SessionId {session.Id}", ip));

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

            if (stripeEvent.Type == Stripe.EventTypes.CheckoutSessionCompleted) //checkout.session.completed
            {
                if (stripeEvent.Data.Object is not Session session || session.Id.Empty()) throw new UnhandledException("stripe session not available");

                if (session.PaymentStatus == "paid" || session.PaymentStatus == "no_payment_required")
                {
                    var principal = await repo.Get<AuthPrincipal>(DocumentType.Principal, session.ClientReferenceId ?? throw new UnhandledException("ClientReferenceId null"), cancellationToken) ??
                        throw new UnhandledException("AuthPrincipal null");

                    var sub = principal.GetSubscription(session.SubscriptionId, PaymentProvider.Stripe);

                    sub.Provider = PaymentProvider.Stripe;

                    sub.Product = AccountProduct.Premium;
                    sub.Cycle = Enum.Parse<AccountCycle>(session.LineItems.Data[0].Price.Metadata["cycle"]);

                    principal.UpdateSubscription(sub);

                    var ip = req.GetUserIP(true);
                    principal.Events.Add(new Event("Stripe (Webhooks) - Checkout", $"New status ({session.Status}) for SubscriptionId ({session.SubscriptionId})", ip));

                    await repo.UpsertItemAsync(principal, cancellationToken);
                }
            }
            else if (stripeEvent.Type.StartsWith("customer.subscription")) //created, updated, deleted, paused, resumed, trial_will_end, pending_update_applied, pending_update_expired
            {
                if (stripeEvent.Data.Object is not Stripe.Subscription subscription || subscription.Id.Empty()) throw new UnhandledException("stripe subscription not available");

                var result = await repo.Query<AuthPrincipal>(x => x.Subscriptions.Any(p => p.CustomerId == subscription.CustomerId), DocumentType.Principal, cancellationToken) ??
                        throw new UnhandledException("AuthPrincipal null");
                var principal = result.LastOrDefault() ?? throw new UnhandledException($"client null - subscription_id:{subscription.Id}");

                var sub = principal.GetSubscription(subscription.Id, PaymentProvider.Stripe);

                sub.Active = subscription.Status is "active" or "trialing";

                sub.Product = AccountProduct.Premium;
                sub.Cycle = Enum.Parse<AccountCycle>(subscription.Items.First().Price.Metadata["cycle"]);

                principal.UpdateSubscription(sub);

                var ip = req.GetUserIP(true);
                principal.Events.Add(new Event("Stripe (Webhooks) - Subscription", $"New status ({subscription.Status}) for SubscriptionId ({subscription.Id})", ip));

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