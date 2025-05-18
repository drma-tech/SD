using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core;

public class PrincipalApi(IHttpClientFactory factory) : ApiCosmos<ClientePrincipal>(factory, "principal")
{
    public async Task<ClientePrincipal?> Get(bool IsUserAuthenticated, bool setNewVersion = false)
    {
        if (IsUserAuthenticated) return await GetAsync(Endpoint.Get, null, setNewVersion);

        return null;
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

    private struct Endpoint
    {
        public const string Get = "principal/get";
        public const string GetEmail = "public/principal/get-email";
        public const string Add = "principal/add";
        public const string Paddle = "principal/paddle";
        public const string Remove = "principal/remove";
    }
}