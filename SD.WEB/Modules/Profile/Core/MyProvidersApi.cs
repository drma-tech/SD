using Microsoft.Extensions.Caching.Memory;
using SD.WEB.Shared;

namespace SD.WEB.Modules.Profile.Core
{
    public class MyProvidersApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCore<MyProviders>(factory, memoryCache, "MyProviders")
    {
        private struct Endpoint
        {
            public const string MyProviders = "MyProviders";
        }

        public async Task<MyProviders?> Get(bool IsUserAuthenticated, RenderControlCore<MyProviders?>? core)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync(Endpoint.MyProviders, core);
            }
            else
            {
                return new();
            }
        }

        public async Task<MyProviders?> Post(MyProviders? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return await PostAsync(Endpoint.MyProviders, null, obj);
        }
    }
}