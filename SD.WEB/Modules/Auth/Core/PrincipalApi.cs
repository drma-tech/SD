using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core;

public class PrincipalApi(IHttpClientFactory factory) : ApiCosmos<AuthPrincipal>(factory, ApiType.Authenticated, "principal")
{
    public async Task<AuthPrincipal?> Get(bool isUserAuthenticated, bool setNewVersion = false)
    {
        if (isUserAuthenticated) return await GetAsync(Endpoint.Get, setNewVersion);

        return null;
    }

    public async Task<AuthPrincipal?> Add(AuthPrincipal? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync(Endpoint.Add, obj);
    }

    public async Task<AuthPrincipal?> Update(AuthPrincipal? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return await PutAsync(Endpoint.Update, obj);
    }

    public async Task Event(string app, string msg)
    {
        ArgumentNullException.ThrowIfNull(msg);

        await PutAsync<AuthPrincipal>(Endpoint.Event(app, msg), null);
    }

    public async Task Remove()
    {
        await DeleteAsync(Endpoint.Remove);
    }

    private struct Endpoint
    {
        public const string Get = "principal/get";
        public const string Add = "principal/add";
        public const string Update = "principal/update";
        public const string Remove = "principal/remove";

        public static string Event(string app, string msg) => $"principal/event?app={app}&msg={msg}";
    }
}

public class PrincipalImportApi(IHttpClientFactory factory) : ApiCosmos<AuthPrincipal>(factory, ApiType.Anonymous, "principal_import")
{
    public async Task<HashSet<AuthPrincipal>> GetAll()
    {
        return await GetListAsync(Endpoint.GetAll);
    }

    public async Task Migrate(string? oldId, string? newId)
    {
        await PutAsync<AuthPrincipal>(Endpoint.Migrate(oldId, newId), null);
    }

    private struct Endpoint
    {
        public const string GetAll = "principal/get-all";

        public static string Migrate(string? oldId, string? newId) => $"principal/migrate/{oldId}/{newId}";
    }
}