using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core;

public class LoginApi(IHttpClientFactory factory) : ApiCosmos<AuthLogin>(factory, null)
{
    public async Task<AuthLogin?> Get(bool isUserAuthenticated)
    {
        if (isUserAuthenticated) return await GetAsync(Endpoint.Get, null);

        return null;
    }

    public async Task Add(SD.Shared.Enums.Platform platform)
    {
        await PostAsync<AuthLogin>(Endpoint.Add(platform.ToString()), null, null);
    }

    private struct Endpoint
    {
        public const string Get = "login/get";

        public static string Add(string platform)
        {
            return $"login/add?platform={platform}";
        }
    }
}
