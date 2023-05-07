using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Provider.Core
{
    public class AllProvidersApi : ApiServices
    {
        public AllProvidersApi(IHttpClientFactory http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string GetAll = "Public/Provider/GetAll";
            public const string Post = "Provider/Post";
            public const string Sync = "Provider/SyncProviders";
        }

        public async Task<AllProviders?> GetAll()
        {
            return await GetAsync<AllProviders>(Endpoint.GetAll, false);
        }

        public async Task<AllProviders?> Post(AllProviders? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.Post, false, obj, Endpoint.GetAll);
        }

        public async Task<AllProviders?> Sync()
        {
            return await PutAsync<AllProviders>(Endpoint.Sync, false, null, Endpoint.GetAll);
        }
    }
}