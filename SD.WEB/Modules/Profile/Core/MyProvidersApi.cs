using SD.Shared.Models.Auth;
using SD.WEB.Shared;

namespace SD.WEB.Modules.Profile.Core
{
    public class MyProvidersApi(IHttpClientFactory factory) : ApiCosmos<MyProviders>(factory)
    {
        private struct Endpoint
        {
            public const string MyProviders = "my-providers";
            public const string MyProvidersAdd = "my-providers/add";
            public const string MyProvidersUpdate = "my-providers/update";
            public const string MyProvidersRemove = "my-providers/remove";
        }

        public async Task<MyProviders?> Get(bool IsUserAuthenticated, RenderControlCore<MyProviders?>? core)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync(Endpoint.MyProviders, core);
            }
            else
            {
                return new();
            }
        }

        public async Task<MyProviders?> Add(MyProviders? obj, MyProvidersItem? item, ClientePaddle? paddle)
        {
            ArgumentNullException.ThrowIfNull(item);
            SubscriptionHelper.ValidateFavoriteProviders(paddle?.ActiveProduct, (obj?.Items.Count ?? 0) + 1);

            return await PostAsync(Endpoint.MyProvidersAdd, null, item);
        }

        public async Task<MyProviders?> Update(MyProviders? obj, ClientePaddle? paddle, bool validatePlan = true)
        {
            ArgumentNullException.ThrowIfNull(obj);
            if (validatePlan) SubscriptionHelper.ValidateFavoriteProviders(paddle?.ActiveProduct, obj.Items.Count + 1);

            return await PostAsync(Endpoint.MyProvidersUpdate, null, obj);
        }

        public async Task<MyProviders?> Remove(MyProvidersItem? item)
        {
            ArgumentNullException.ThrowIfNull(item);

            return await PostAsync(Endpoint.MyProvidersRemove, null, item);
        }
    }
}