using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Core.Models;
using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core
{
    public class PrincipalApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiServices(factory, memoryCache)
    {
        private struct Endpoint
        {
            public const string Get = "Principal/Get";
            public const string GetEmail = "Public/Principal/GetEmail";
            public const string Add = "Principal/Add";
            public const string Paddle = "Principal/Paddle";
            public const string Remove = "Principal/Remove";
        }

        public async Task<ClientePrincipal?> Get(bool IsUserAuthenticated)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync<ClientePrincipal>(Endpoint.Get, false);
            }
            else
            {
                return null;
            }
        }

        public async Task<string?> GetEmail(string? token)
        {
            return await GetValueAsync($"{Endpoint.GetEmail}?token={token}", false);
        }

        public async Task<Gravatar?> GetGravatar(string? email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            Gravatar? result = null;
            try
            {
                result = MemoryCache.Get<Gravatar>("empty-gravatar");

                if (result == null)
                {
                    var root = await GetAsync<GravatarRoot>($"https://en.gravatar.com/{email.GenerateHash()}.json", true);
                    result = root?.entry.LastOrDefault();
                }
            }
            catch (Exception)
            {
                result = new()
                {
                    displayName = email.Split("@")[0],
                    photos = [new Photo { value = $"https://en.gravatar.com/avatar/{email.GenerateHash()}?d=retro" }]
                };

                MemoryCache.Set("empty-gravatar", result);
            }
            return result;
        }

        public async Task<ClientePrincipal?> Add(ClientePrincipal? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return await PostAsync<ClientePrincipal>(Endpoint.Add, false, obj, Endpoint.Get);
        }

        public async Task<ClientePrincipal?> Paddle(ClientePrincipal? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return await PutAsync<ClientePrincipal>(Endpoint.Paddle, false, obj, Endpoint.Get);
        }

        public async Task Remove()
        {
            await DeleteAsync(Endpoint.Remove, false, Endpoint.Get);
        }
    }
}