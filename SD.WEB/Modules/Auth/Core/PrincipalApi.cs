using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core;

public class PrincipalApi(IHttpClientFactory factory) : ApiCosmos<AuthPrincipal>(factory, ApiType.Authenticated, "principal", ApiContext.Default.AuthPrincipal)
{
    public async Task<AuthPrincipal?> Get(bool setNewVersion = false, CancellationToken cancellationToken = default)
    {
        return await GetAsync(Endpoint.Get, setNewVersion, null, cancellationToken);
    }

    public async Task<AuthPrincipal?> Add(AuthPrincipal? obj, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync(Endpoint.Add, obj, cancellationToken);
    }

    public async Task<AuthPrincipal?> Update(AuthPrincipal? obj, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return await PutAsync(Endpoint.Update, obj, ApiContext.Default.AuthPrincipal, cancellationToken);
    }

    public async Task Event(string app, string msg, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(msg);

        await PutAsync(Endpoint.Event(app, msg), null, ApiContext.Default.AuthPrincipal, cancellationToken);
    }

    public async Task Remove(CancellationToken cancellationToken)
    {
        await DeleteAsync(Endpoint.Remove, cancellationToken);
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

public class PrincipalImportApi(IHttpClientFactory factory) : ApiCosmos<AuthPrincipal>(factory, ApiType.Anonymous, "principal_import", ApiContext.Default.AuthPrincipal)
{
    public async Task<HashSet<AuthPrincipal>> GetAll(CancellationToken cancellationToken)
    {
        return await GetListAsync(Endpoint.GetAll, null, cancellationToken);
    }

    public async Task Migrate(string? oldId, string? newId, CancellationToken cancellationToken)
    {
        await PutAsync(Endpoint.Migrate(oldId, newId), null, ApiContext.Default.AuthPrincipal, cancellationToken);
    }

    private struct Endpoint
    {
        public const string GetAll = "principal/get-all";

        public static string Migrate(string? oldId, string? newId) => $"principal/migrate/{oldId}/{newId}";
    }
}