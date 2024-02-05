using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core
{
    public class PrincipalApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCore<ClientePrincipal>(factory, memoryCache, "ClientePrincipal")
    {
        private struct Endpoint
        {
            public const string Get = "Principal/Get";
            public const string GetEmail = "Public/Principal/GetEmail";
            public const string Add = "Principal/Add";
            public const string Paddle = "Principal/Paddle";
            public const string Remove = "Principal/Remove";
        }

        public async Task<ClientePrincipal?> Get(bool IsUserAuthenticated, bool forceClean = false)
        {
            if (forceClean)
            {
                CleanCache();
            }

            if (IsUserAuthenticated)
            {
                return await GetAsync(Endpoint.Get, null);
            }
            else
            {
                return null;
            }
        }

        public async Task<string?> GetEmail(string? token)
        {
            return await GetValueAsync($"{Endpoint.GetEmail}?token={token}", null);
        }

        public async Task<ClientePrincipal?> Add(ClientePrincipal? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return await PostAsync(Endpoint.Add, null, obj);
        }

        public async Task<ClientePrincipal?> Paddle(ClientePrincipal? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return await PutAsync(Endpoint.Paddle, null, obj);
        }

        public async Task Remove()
        {
            await DeleteAsync(Endpoint.Remove, null);
        }
    }
}