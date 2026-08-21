using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.Shared.Core.Types;
using SD.Shared.Models.Auth;
using System.Net;
using System.Net.Http.Json;

namespace SD.API.Functions.Webhook;

public class PaymentWebhookFunction(CosmosMainRepository repo)
{
    private const string APP_CODE = "sd";
    private const string APP = "app";
    private const string USERID = "userId";

    [Function("PostAppleWebhook")]
    public async Task PostAppleWebhook(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/apple/webhook")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var body = await req.ReadFromJsonAsync<Dictionary<string, string>>(cancellationToken) ?? throw new UnhandledException("body null");

        if (!body.TryGetValue("signedPayload", out var signedPayload)) throw new UnhandledException("signedPayload null");

        var notification = AppleJwtDecoder.DecodeServerNotification(signedPayload, ApiStartup.Configurations.Apple!);

        var info = notification.Data;

        var transaction = AppleJwtDecoder.DecodeTransaction(info.SignedTransactionInfo);

        var originalTransactionId = transaction.OriginalTransactionId;

        var results = await repo.Query<AuthPrincipal>(MainType.Principal, x => x.Subscriptions.Any(p => p.SubscriptionId == originalTransactionId), transform: null, cancellationToken);

        var client = results.LastOrDefault() ?? throw new UnhandledException($"client null - originalTransactionId:{originalTransactionId}");
        var sub = client.GetSubscription(originalTransactionId, PaymentProvider.Apple);

        if (string.Equals(notification.NotificationType, "REFUND", StringComparison.OrdinalIgnoreCase) || string.Equals(notification.NotificationType, "REVOKE", StringComparison.OrdinalIgnoreCase))
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
        sub.Cycle = product.Contains("yearly", StringComparison.OrdinalIgnoreCase) ? AccountCycle.Yearly : AccountCycle.Monthly;

        client.UpdateSubscription(sub);

        var ip = req.GetUserIP(includePort: true);
        client.Events.Add(new Event("Apple (Webhooks)", $"SubscriptionId = {originalTransactionId}, Cycle = {sub.Cycle}, Type = {notification.NotificationType}, Subtype = {notification.Subtype}, expiresDate = {sub.ExpiresDate}", ip));

        await repo.UpsertItemAsync(client);
    }

    [Function("PostStripeWebhook")]
    public async Task<HttpResponseData> PostStripeWebhook(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/stripe/webhook")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var json = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);

        req.Headers.TryGetValues("Stripe-Signature", out var Signature);
        if (string.IsNullOrEmpty(Signature?.First())) throw new UnhandledException("Stripe signature missing");
        var stripeEvent = Stripe.EventUtility.ConstructEvent(json, Signature?.First(), ApiStartup.Configurations.Stripe?.SigningSecret ?? throw new UnhandledException("Stripe SigningSecret not configured"), throwOnApiVersionMismatch: false);

        if (stripeEvent.Type.StartsWith("customer.subscription", StringComparison.OrdinalIgnoreCase)) //created, updated, deleted, paused, resumed, trial_will_end, pending_update_applied, pending_update_expired
        {
            if (stripeEvent.Data.Object is not Stripe.Subscription obj || obj.Id.Empty()) throw new UnhandledException("stripe subscription not available");

            if (!obj.Metadata.TryGetValue(APP, out var app) || !string.Equals(app, APP_CODE, StringComparison.OrdinalIgnoreCase))
                return await req.CreateResponse(HttpStatusCode.OK, $"webhook ignored -> app={app ?? "null"}", cancellationToken);

            if (!obj.Metadata.TryGetValue(USERID, out var userId) || userId.Empty())
                throw new UnhandledException("userId metadata missing in session");

            var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken) ?? throw new UnhandledException($"stripe webhook - principal is null - subscriptionId:{obj.Id}");
            var sub = principal.GetSubscription(obj.Id, PaymentProvider.Stripe);

            sub.Active = obj.Status is "active" or "trialing";

            sub.Cycle = obj.Items.First().Price.Metadata["cycle"].ParseToEnum<AccountCycle>(AccountCycle.Monthly);

            if (obj.CancelAt.HasValue)
            {
                sub.ExpiresDate = new DateTimeOffset(obj.CancelAt.Value);
            }

            principal.UpdateSubscription(sub);

            var ip = req.GetUserIP(includePort: true);
            var type = stripeEvent.Type.Split(".")[2];
            principal.Events.Add(new Event("Stripe (Webhooks)", $"Type = {type}, Status = {obj.Status}, Cycle = {sub.Cycle} for SubscriptionId = {obj.Id}", ip));

            await repo.UpsertItemAsync(principal);
        }
        else if (string.Equals(stripeEvent.Type, "customer.deleted", StringComparison.OrdinalIgnoreCase))
        {
            if (stripeEvent.Data.Object is not Stripe.Customer obj || obj.Id.Empty()) throw new UnhandledException("stripe customer not available");

            if (!obj.Metadata.TryGetValue("app", out var app) || !string.Equals(app, APP_CODE, StringComparison.OrdinalIgnoreCase))
                return await req.CreateResponse(HttpStatusCode.OK, $"webhook ignored -> app={app ?? "null"}", cancellationToken);

            if (!obj.Metadata.TryGetValue(USERID, out var userId) || userId.Empty())
            {
                //if no metadada, try to find the user with the StripeCustomerId
                var list = await repo.Query<AuthPrincipal>(MainType.Principal, p => p.StripeCustomerId == obj.Id, transform: null, cancellationToken);

                if (list.Count > 0)
                {
                    var item = list.First();
                    item.StripeCustomerId = null;
                    await repo.UpsertItemAsync(item);

                    return await req.CreateResponse(HttpStatusCode.OK, "userId metadata missing", cancellationToken);
                }

                throw new UnhandledException("stripe customer id not available");
            }

            var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken);

            if (principal != null)
            {
                principal.StripeCustomerId = null;
                await repo.UpsertItemAsync(principal);
            }
        }

        return await req.CreateResponse(HttpStatusCode.OK, "webhook received", cancellationToken);
    }
}