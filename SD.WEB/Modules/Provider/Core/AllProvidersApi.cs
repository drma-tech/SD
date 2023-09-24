using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace SD.WEB.Modules.Provider.Core
{
    public class AllProvidersApi : ApiServices
    {
        public IHttpClientFactory _factory { get; set; }

        public AllProvidersApi(IHttpClientFactory http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
            _factory = http;
        }

        private struct Endpoint
        {
            public const string GetAll = "Public/Provider/GetAll";
            public const string Post = "Provider/Post";
            public const string Sync = "Provider/SyncProviders";
        }

        public async Task<AllProviders?> GetAll()
        {
            var http = _factory.CreateClient("RetryHttpClient");
            return await http.GetFromJsonAsync<AllProviders>("/Data/providers.json");
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