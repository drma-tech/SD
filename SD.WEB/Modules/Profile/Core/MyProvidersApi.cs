using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Profile.Core
{
    public class MyProvidersApi : ApiServices
    {
        public MyProvidersApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string MyProviders = "MyProviders";
        }

        public async Task<MyProviders?> Get(bool IsUserAuthenticated)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync<MyProviders>(Endpoint.MyProviders, false);
            }
            else
            {
                return new();
            }
        }

        public async Task<MyProviders?> Post(MyProviders? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.MyProviders, false, obj, Endpoint.MyProviders);
        }
    }
}