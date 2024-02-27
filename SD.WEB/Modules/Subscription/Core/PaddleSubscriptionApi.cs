using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Profile.Core
{
    public class PaddleSubscriptionApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCore<RootSubscription>(factory, memoryCache, "PaddleSubscriptionApi")
    {
        private struct Endpoint
        {
            public static string subscription(string? id) => $"public/paddle/subscription?id={id}";

            public static string subscriptionUpdate(string? id) => $"public/paddle/subscription/update?id={id}";
        }

        public async Task<RootSubscription?> GetSubscription(string? id, bool forceClean = false)
        {
            if (forceClean)
            {
                CleanCache();
            }

            return await GetAsync(Endpoint.subscription(id), null);
        }

        public async Task<RootSubscription?> GetSubscriptionUpdate(string? id)
        {
            return await GetAsync(Endpoint.subscriptionUpdate(id), null);
        }
    }
}