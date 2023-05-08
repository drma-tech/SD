namespace SD.WEB.Modules.Profile.Core
{
    public class WatchedListApi : ApiServices
    {
        public WatchedListApi(HttpClient http, Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string Get = "WatchedList/Get";

            public static string Add(MediaType? type, string TmdbId) => $"WatchedList/Add/{type}/{TmdbId}";

            public static string Remove(MediaType? type, string TmdbId) => $"WatchedList/Remove/{type}/{TmdbId}";
        }

        public async Task<WatchedList?> Get(bool IsUserAuthenticated)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync<WatchedList>(Endpoint.Get, false);
            }
            else
            {
                return default;
            }
        }

        public async Task<WatchedList?> Add(MediaType? mediaType, string? TmdbId)
        {
            if (mediaType == null) throw new ArgumentNullException(nameof(mediaType));
            if (TmdbId == null) throw new ArgumentNullException(nameof(TmdbId));

            return await PostAsync<WatchedList>(Endpoint.Add(mediaType, TmdbId), false, null, Endpoint.Get);
        }

        public async Task<WatchedList?> Remove(MediaType? mediaType, string? TmdbId)
        {
            if (mediaType == null) throw new ArgumentNullException(nameof(mediaType));
            if (TmdbId == null) throw new ArgumentNullException(nameof(TmdbId));

            return await PostAsync<WatchedList>(Endpoint.Remove(mediaType, TmdbId), false, null, Endpoint.Get);
        }
    }
}