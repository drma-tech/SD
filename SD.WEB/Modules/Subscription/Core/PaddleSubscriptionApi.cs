using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Profile.Core
{
    public class PaddleSubscriptionApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCore<SubscriptionRoot>(factory, memoryCache, "PaddleSubscriptionApi")
    {
        private struct Endpoint
        {
            public static string subscription(string? id) => $"public/paddle/subscription?id={id}";
        }

        public async Task<SubscriptionRoot?> GetSubscription(string? id)
        {
            return await GetAsync(Endpoint.subscription(id), null);
        }
    }
}