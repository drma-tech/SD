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

        public async Task<AllProviders?> GetAll()
        {
            var http = _factory.CreateClient("RetryHttpClient");
            return await http.GetFromJsonAsync<AllProviders>("/data/providers.json");
        }
    }
}