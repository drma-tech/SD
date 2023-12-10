using Microsoft.Extensions.Caching.Memory;

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

        public async Task<SD.Shared.Models.MySuggestions?> Get(bool IsUserAuthenticated)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync(Endpoint.Get);
            }
            else
            {
                return new();
            }
        }

        public async Task<SD.Shared.Models.MySuggestions?> Sync(MediaType? mediaType, SD.Shared.Models.MySuggestions obj)
        {
            ArgumentNullException.ThrowIfNull(mediaType);
            ArgumentNullException.ThrowIfNull(obj);

            return await PostAsync(Endpoint.Sync(mediaType), obj);
        }

        public async Task Add(SD.Shared.Models.MySuggestions obj)
        {
            await PostAsync(Endpoint.Add, obj);
        }
    }
}