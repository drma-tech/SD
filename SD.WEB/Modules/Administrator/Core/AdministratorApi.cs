using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Auth.Core
{
    public class AdministratorApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiServices(factory, memoryCache)
    {
        private struct Endpoint
        {
            public const string Add = "adm/bla/test";
        }

        public async Task<string?> Test()
        {
            return await GetValueAsync(Endpoint.Add, false);
        }
    }
}