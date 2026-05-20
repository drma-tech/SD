using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core;

public class LoginApi(IHttpClientFactory factory) : ApiCosmos<AuthLogin>(factory, ApiType.Authenticated, null)
{
    public async Task<AuthLogin?> Get(bool isUserAuthenticated)
    {
        if (isUserAuthenticated) return await GetAsync(Endpoint.Get);

        return null;
    }

    public async Task Add(SD.Shared.Enums.Platform platform, string? country)
    {
        await PostAsync<AuthLogin>(Endpoint.Add(platform.ToString(), country), null);
    }

    private struct Endpoint
    {
        public const string Get = "login/get";

        public static string Add(string platform, string? country)
        {
            return $"login/add?platform={platform}&country={(country ?? "error")}";
        }
    }
}

public class PublicLoginApi(IHttpClientFactory factory) : ApiCosmos<AuthLogin>(factory, ApiType.Anonymous, null)
{
    public async Task SendEmail(string? email, string? reference)
    {
        await PostAsync<AuthLogin>(Endpoint.SendEmail(email, reference), null);
    }

    public async Task<string?> StatusEmail(string? reference)
    {
        return await GetStringAsync(Endpoint.StatusEmail(reference));
    }

    private struct Endpoint
    {
        public static string SendEmail(string? email, string? reference)
        {
            return $"public/login/email?email={email}&reference={(reference ?? "error")}";
        }

        public static string StatusEmail(string? reference)
        {
            return $"public/login/status?reference={(reference ?? "error")}";
        }
    }
}
