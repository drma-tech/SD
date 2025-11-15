using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core;

public class PrincipalApi(IHttpClientFactory factory) : ApiCosmos<AuthPrincipal>(factory, ApiType.Authenticated, "principal")
{
    public async Task<AuthPrincipal?> Get(bool isUserAuthenticated, bool setNewVersion = false)
    {
        if (isUserAuthenticated) return await GetAsync(Endpoint.Get, null, setNewVersion);

        return null;
    }

    public async Task<AuthPrincipal?> Add(AuthPrincipal? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync(Endpoint.Add, null, obj);
    }

    public async Task<AuthPrincipal?> Update(AuthPrincipal? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return await PutAsync(Endpoint.Update, null, obj);
    }

    public async Task Event(string msg)
    {
        ArgumentNullException.ThrowIfNull(msg);

        await PutAsync<AuthPrincipal>(Endpoint.Event(msg), null, null);
    }

    public async Task Remove()
    {
        await DeleteAsync(Endpoint.Remove, null);
    }

    private struct Endpoint
    {
        public const string Get = "principal/get";
        public const string Add = "principal/add";
        public const string Update = "principal/update";
        public const string Remove = "principal/remove";

        public static string Event(string msg) => $"principal/event?msg={msg}";
    }
}