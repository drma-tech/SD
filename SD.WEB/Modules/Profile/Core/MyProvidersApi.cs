using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Profile.Core
{
    public class MyProvidersApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCore<MyProviders>(factory, memoryCache, "MyProviders")
    {
        private struct Endpoint
        {
            public const string MyProviders = "MyProviders";
        }

        public async Task<MyProviders?> Get(bool IsUserAuthenticated)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync(Endpoint.MyProviders);
            }
            else
            {
                return new();
            }
        }

        public async Task<MyProviders?> Post(MyProviders? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return await PostAsync(Endpoint.MyProviders, obj);
        }
    }
}