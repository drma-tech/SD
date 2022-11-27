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
            public const string Get = "MyProviders/Get";
            public const string Post = "MyProviders/Post";
        }

        public async Task<MyProviders?> Get(bool IsUserAuthenticated)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync<MyProviders>(Endpoint.Get, false);
            }
            else
            {
                return new();
            }
        }

        public async Task<MyProviders?> Post(MyProviders? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.Post, false, obj, Endpoint.Get);
        }
    }
}