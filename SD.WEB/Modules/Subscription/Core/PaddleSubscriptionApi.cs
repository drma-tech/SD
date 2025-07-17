using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Core;

public class PaddleSubscriptionApi(IHttpClientFactory factory) : ApiCosmos<RootSubscription>(factory, null)
{
    public async Task<RootSubscription?> GetSubscription(string? id)
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