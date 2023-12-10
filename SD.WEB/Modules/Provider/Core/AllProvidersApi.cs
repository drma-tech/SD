using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace SD.WEB.Modules.Provider.Core
{
    public class AllProvidersApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCore<AllProviders>(factory, memoryCache, "AllProviders")
    {
        public async Task<AllProviders?> GetAll()
        {
            return await _http.GetFromJsonAsync<AllProviders>("/data/providers.json");
        }
    }
}