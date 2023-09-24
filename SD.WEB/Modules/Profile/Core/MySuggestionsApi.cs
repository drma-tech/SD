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
            public const string Add = "MySuggestions/Add";

            public static string Sync(MediaType? type) => $"MySuggestions/Sync/{type}";
        }

        public async Task<SD.Shared.Models.MySuggestions?> Get()
        {
            return await GetAsync<SD.Shared.Models.MySuggestions>(Endpoint.Get, false);
        }

        public async Task<SD.Shared.Models.MySuggestions?> Sync(MediaType? mediaType, SD.Shared.Models.MySuggestions obj)
        {
            if (mediaType == null) throw new ArgumentNullException(nameof(mediaType));

            return await PostAsync(Endpoint.Sync(mediaType), false, obj, Endpoint.Get);
        }

        public async Task Add(SD.Shared.Models.MySuggestions obj)
        {
            await PostAsync(Endpoint.Add, false, obj, Endpoint.Get);
        }
    }
}