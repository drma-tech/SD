namespace SD.WEB.Modules.Profile.Core;

public class MyProvidersApi(IHttpClientFactory factory) : ApiCosmos<MyProviders>(factory, ApiType.Authenticated, "my-providers")
{
    public async Task<MyProviders?> Get(bool isUserAuthenticated)
    {
        if (isUserAuthenticated) return await GetAsync(Endpoint.MyProviders);

        return new MyProviders();
    }

    public async Task<MyProviders?> Add(MyProviders? obj, MyProvidersItem? item, AccountProduct? product)
    {
        ArgumentNullException.ThrowIfNull(item);
        SubscriptionHelper.ValidateFavoriteProviders(product, (obj?.Items.Count ?? 0) + 1);

        return await PostAsync(Endpoint.MyProvidersAdd, item);
    }

    public async Task<MyProviders?> Update(MyProviders? obj, AccountProduct? product, bool validatePlan = true)
    {
        ArgumentNullException.ThrowIfNull(obj);
        if (validatePlan) SubscriptionHelper.ValidateFavoriteProviders(product, obj.Items.Count + 1);

        return await PostAsync(Endpoint.MyProvidersUpdate, obj);
    }

    public async Task<MyProviders?> Remove(MyProvidersItem? item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return await PostAsync(Endpoint.MyProvidersRemove, item);
    }

    private struct Endpoint
    {
        public const string MyProviders = "my-providers";
        public const string MyProvidersAdd = "my-providers/add";
        public const string MyProvidersUpdate = "my-providers/update";
        public const string MyProvidersRemove = "my-providers/remove";
    }
}