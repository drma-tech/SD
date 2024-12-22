using SD.WEB.Shared;

namespace SD.WEB.Modules.Profile.Core
{
    public class MySuggestionsApi(IHttpClientFactory factory) : ApiCosmos<SD.Shared.Models.MySuggestions>(factory, "my-suggestions")
    {
        private struct Endpoint
        {
            public const string Get = "my-suggestions/get";
            public const string Add = "my-suggestions/add";

            public static string Sync(MediaType? type) => $"my-suggestions/sync/{type}";
        }

        public async Task<SD.Shared.Models.MySuggestions?> Get(AccountProduct? product, bool IsUserAuthenticated, RenderControlCore<SD.Shared.Models.MySuggestions?>? core)
        {
            if (product is null or AccountProduct.Basic)
            {
                return new();
            }
            else if (IsUserAuthenticated)
            {
                return await GetAsync(Endpoint.Get, core);
            }
            else
            {
                return new();
            }
        }

        public async Task<SD.Shared.Models.MySuggestions?> Sync(MediaType? mediaType, SD.Shared.Models.MySuggestions obj, RenderControlCore<SD.Shared.Models.MySuggestions?>? core)
        {
            ArgumentNullException.ThrowIfNull(mediaType);
            ArgumentNullException.ThrowIfNull(obj);

            return await PostAsync(Endpoint.Sync(mediaType), core, obj);
        }

        public async Task<SD.Shared.Models.MySuggestions?> Add(SD.Shared.Models.MySuggestions obj)
        {
            return await PostAsync(Endpoint.Add, null, obj);
        }
    }
}