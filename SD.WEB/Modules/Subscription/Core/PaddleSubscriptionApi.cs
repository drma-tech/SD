using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Core
{
    public class PaddleSubscriptionApi(IHttpClientFactory factory) : ApiCosmos<RootSubscription>(factory, null)
    {
        private struct Endpoint
        {
            public static string subscription(string? id) => $"public/paddle/subscription?id={id}";

            public static string subscriptionUpdate(string? id) => $"public/paddle/subscription/update?id={id}";
        }

        public async Task<RootSubscription?> GetSubscription(string? id)
        {
            return await GetAsync(Endpoint.subscription(id), null);
        }

        public async Task<RootSubscription?> GetSubscriptionUpdate(string? id)
        {
            return await GetAsync(Endpoint.subscriptionUpdate(id), null);
        }
    }
}