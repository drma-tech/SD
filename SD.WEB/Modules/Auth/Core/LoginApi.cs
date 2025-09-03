using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core;

public class LoginApi(IHttpClientFactory factory) : ApiCosmos<AuthLogin>(factory, null)
{
    public async Task<AuthLogin?> Get(bool isUserAuthenticated)
    {
        if (isUserAuthenticated) return await GetAsync(Endpoint.Get, null);

        return null;
    }

    public async Task Add(string platform)
    {
        await PostAsync<AuthLogin>(Endpoint.Add(platform), null, null);
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
