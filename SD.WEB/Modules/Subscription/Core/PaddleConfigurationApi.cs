using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Core
{
    public class PaddleConfigurationApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCosmos<Configurations>(factory, memoryCache, "PaddleConfigurationApi")
    {
        private struct Endpoint
        {
            public const string configurations = "public/paddle/configurations";
        }

        public async Task<Configurations?> GetConfigurations()
        {
            return await GetAsync(Endpoint.configurations, null);
        }
    }
}