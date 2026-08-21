using SD.WEB.Api.Core;
using System.Text.Json.Serialization.Metadata;

namespace SD.WEB.Api.Module.Cosmos.Authenticated;

public class MyProvidersApi(IHttpClientFactory factory) : ApiCosmos<MyProviders>(factory, ApiType.Authenticated, "my-providers", [], ApiContext.Default.MyProviders)
{
    public async Task<MyProviders?> Get(RenderControlState<MyProviders>? actions, CancellationToken cancellationToken)
    {
        if (!AppStateStatic.IsAuthenticated) return default;

        return await GetAsync("my-providers", setNewVersion: true, actions, cancellationToken);
    }

    public async Task<MyProviders?> Add(MyProviders? obj, MyProvidersItem? item, RenderControlState<MyProviders>? state, AccountProduct? product, JsonTypeInfo<MyProvidersItem?> requestTypeInfo, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(item);
        SubscriptionHelper.ValidateFavoriteProviders(product, (obj?.Items.Count ?? 0) + 1);

        return await PostAsync("my-providers/add", item, requestTypeInfo, state, cancellationToken);
    }

    public async Task<MyProviders?> Update(MyProviders? obj, RenderControlState<MyProviders>? state, AccountProduct? product, bool validatePlan, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (validatePlan) SubscriptionHelper.ValidateFavoriteProviders(product, obj.Items.Count + 1);

        return await PostAsync("my-providers/update", obj, state, cancellationToken);
    }

    public async Task<MyProviders?> Remove(MyProvidersItem? item, RenderControlState<MyProviders>? state, JsonTypeInfo<MyProvidersItem?> requestTypeInfo, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(item);

        return await PostAsync("my-providers/remove", item, requestTypeInfo, state, cancellationToken);
    }
}