using Microsoft.Extensions.Caching.Memory;
using SD.WEB.Shared;

namespace SD.WEB.Modules.Profile.Core
{
    public class MySuggestionsApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCore<SD.Shared.Models.MySuggestions>(factory, memoryCache, "MySuggestions")
    {
        private struct Endpoint
        {
            public const string Get = "MySuggestions/Get";
            public const string Add = "MySuggestions/Add";

            public static string Sync(MediaType? type) => $"MySuggestions/Sync/{type}";
        }

        public async Task<SD.Shared.Models.MySuggestions?> Get(bool IsUserAuthenticated, RenderControlCore<SD.Shared.Models.MySuggestions?>? core)
        {
            if (IsUserAuthenticated)
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