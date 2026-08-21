using SD.Shared.Models.Auth;
using SD.WEB.Api.Core;

namespace SD.WEB.Api.Module.Cosmos.Authenticated;

public class PrincipalApi(IHttpClientFactory factory) : ApiCosmos<AuthPrincipal>(factory, ApiType.Authenticated, "principal", [], ApiContext.Default.AuthPrincipal)
{
    public async Task<AuthPrincipal?> Get(bool setNewVersion = false, CancellationToken cancellationToken = default)
    {
        if (!AppStateStatic.IsAuthenticated) return default;

        return await GetAsync("principal/get", setNewVersion, state: null, cancellationToken);
    }

    public async Task<AuthPrincipal> Add(AuthPrincipal? obj, Platform platform, string? country, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync($"principal/add?platform={platform}&country={country}", obj, state: null, cancellationToken);
    }

    public async Task<AuthPrincipal> Update(AuthPrincipal? obj, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return await PutAsync("principal/update", obj, ApiContext.Default.AuthPrincipal, state: null, cancellationToken);
    }

    public async Task<AuthPrincipal> Event(string app, string msg, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(msg);

        return await PostAsync($"principal/event?app={app}&msg={msg}", null, state: null, cancellationToken);
    }

    public async Task Remove(CancellationToken cancellationToken)
    {
        await DeleteAsync("principal/remove", cancellationToken);
    }

    public async Task<AuthPrincipal?> StripeCustomer(CancellationToken cancellationToken)
    {
        return await GetAsync("stripe/customer", setNewVersion: true, state: null, cancellationToken);
    }
}