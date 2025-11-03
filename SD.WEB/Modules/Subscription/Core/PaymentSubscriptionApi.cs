using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Core;

public class PaymentSubscriptionApi(IHttpClientFactory factory) : ApiCosmos<PaddleRootSubscription>(factory, ApiType.Anonymous, null)
{
    public async Task<PaddleRootSubscription?> GetSubscription(string? id)
    {
        return await GetAsync(Endpoint.Subscription(id), null);
    }

    private struct Endpoint
    {
        public static string Subscription(string? id)
        {
            return $"public/paddle/subscription?id={id}";
        }
    }
}