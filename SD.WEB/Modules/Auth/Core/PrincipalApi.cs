using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core
{
    public class PrincipalApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiServices(factory, memoryCache)
    {
        private struct Endpoint
        {
            public const string Get = "Principal/Get";
            public const string Add = "Principal/Add";
            public const string Remove = "Principal/Remove";
        }

        public async Task<ClientePrincipal?> Get()
        {
            return await GetAsync<ClientePrincipal>(Endpoint.Get, false);
        }

        public async Task<ClientePrincipal?> Add(ClientePrincipal? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return await PostAsync<ClientePrincipal>(Endpoint.Add, false, obj, Endpoint.Get);
        }

        public async Task Remove()
        {
            await DeleteAsync(Endpoint.Remove, false, Endpoint.Get);
        }
    }
}