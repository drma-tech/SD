using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core
{
    public class LoginApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiServices(factory, memoryCache)
    {
        private struct Endpoint
        {
            public static string Add(string platform) => $"login/add?platform={platform}";
        }

        public async Task Add(string platform)
        {
            await PostAsync<ClienteLogin>(Endpoint.Add(platform), false, null, null);
        }
    }
}