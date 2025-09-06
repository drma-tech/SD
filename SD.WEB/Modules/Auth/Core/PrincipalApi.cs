using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core;

public class PrincipalApi(IHttpClientFactory factory) : ApiCosmos<AuthPrincipal>(factory, "principal")
{
    public async Task<AuthPrincipal?> Get(bool isUserAuthenticated, bool setNewVersion = false)
    {
        if (isUserAuthenticated) return await GetAsync(Endpoint.Get, null, setNewVersion);

        return null;
    }

    public async Task<string?> GetEmail(string? token)
    {
        return await GetValueAsync($"{Endpoint.GetEmail}?token={token}", null);
    }

    public async Task<AuthPrincipal?> Add(AuthPrincipal? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync(Endpoint.Add, null, obj);
    }

    public async Task Event(string msg)
    {
        ArgumentNullException.ThrowIfNull(msg);

        await PutAsync<AuthPrincipal>(Endpoint.Event(msg), null, null);
    }

    public async Task<AuthPrincipal?> Paddle(AuthPrincipal? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return await PutAsync(Endpoint.Paddle, null, obj);
    }

    public async Task Remove()
    {
        await DeleteAsync(Endpoint.Remove, null);
    }

    private struct Endpoint
    {
        public const string Get = "principal/get";
        public const string GetEmail = "public/principal/get-email";
        public const string Add = "principal/add";
        public const string Paddle = "principal/paddle";
        public const string Remove = "principal/remove";

        public static string Event(string msg) => $"principal/event?msg={msg}";
    }
}
