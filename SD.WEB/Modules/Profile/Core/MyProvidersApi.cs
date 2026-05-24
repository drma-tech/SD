using System.Text.Json.Serialization.Metadata;

namespace SD.WEB.Modules.Profile.Core;

public class MyProvidersApi(IHttpClientFactory factory) : ApiCosmos<MyProviders>(factory, ApiType.Authenticated, "my-providers", ApiContext.Default.MyProviders)
{
    public async Task<MyProviders?> Get(bool isUserAuthenticated, CancellationToken cancellationToken)
    {
        if (isUserAuthenticated) return await GetAsync(Endpoint.MyProviders, true, cancellationToken);

        return new MyProviders();
    }

    public async Task<MyProviders?> Add(MyProviders? obj, MyProvidersItem? item, AccountProduct? product, JsonTypeInfo<MyProvidersItem?> requestTypeInfo, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(item);
        SubscriptionHelper.ValidateFavoriteProviders(product, (obj?.Items.Count ?? 0) + 1);

        return await PostAsync(Endpoint.MyProvidersAdd, item, requestTypeInfo, cancellationToken);
    }

    public async Task<MyProviders?> Update(MyProviders? obj, AccountProduct? product, bool validatePlan, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (validatePlan) SubscriptionHelper.ValidateFavoriteProviders(product, obj.Items.Count + 1);

        return await PostAsync(Endpoint.MyProvidersUpdate, obj, cancellationToken);
    }

    public async Task<MyProviders?> Remove(MyProvidersItem? item, JsonTypeInfo<MyProvidersItem?> requestTypeInfo, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(item);

        return await PostAsync(Endpoint.MyProvidersRemove, item, requestTypeInfo, cancellationToken);
    }

    private struct Endpoint
    {
        public const string MyProviders = "my-providers";
        public const string MyProvidersAdd = "my-providers/add";
        public const string MyProvidersUpdate = "my-providers/update";
        public const string MyProvidersRemove = "my-providers/remove";
    }
}