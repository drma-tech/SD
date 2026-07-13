using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core;

public class LoginApi(IHttpClientFactory factory) : ApiCosmos<AuthLogin>(factory, ApiType.Authenticated, null, ApiContext.Default.AuthLogin)
{
    public async Task<AuthLogin?> Get(CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.Get, true, null, cancellationToken);
    }

    public async Task Add(SD.Shared.Enums.Platform platform, string? country, CancellationToken cancellationToken)
    {
        await PostAsync(Endpoint.Add(platform.ToString(), country), null, ApiContext.Default.AuthLogin, cancellationToken);
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

public class PublicLoginApi(IHttpClientFactory factory) : ApiCosmos<AuthLogin>(factory, ApiType.Anonymous, null, ApiContext.Default.AuthLogin)
{
    public async Task SendEmail(string? email, string? reference, CancellationToken cancellationToken)
    {
        await PostAsync(Endpoint.SendEmail(email, reference), null, ApiContext.Default.AuthLogin, cancellationToken);
    }

    public async Task<string?> StatusEmail(string? reference, CancellationToken cancellationToken)
    {
        return await GetStringAsync(Endpoint.StatusEmail(reference), cancellationToken);
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
