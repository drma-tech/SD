using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Profile.Core
{
    public class MySuggestionsApi : ApiServices
    {
        public MySuggestionsApi(IHttpClientFactory http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string Get = "MySuggestions/Get";

            public static string Sync(MediaType? type) => $"MySuggestions/Sync/{type}";
        }

        public async Task<SD.Shared.Models.MySuggestions?> Get(bool IsUserAuthenticated)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync<SD.Shared.Models.MySuggestions>(Endpoint.Get, false);
            }
            else
            {
                return default;
            }
        }

        public async Task<SD.Shared.Models.MySuggestions?> Sync(MediaType? mediaType, SD.Shared.Models.MySuggestions obj)
        {
            if (mediaType == null) throw new ArgumentNullException(nameof(mediaType));

            return await PostAsync(Endpoint.Sync(mediaType), false, obj);
        }
    }
}