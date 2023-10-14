using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Profile.Core
{
    public class WatchingListApi : ApiServices
    {
        public WatchingListApi(IHttpClientFactory http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string Get = "WatchingList/Get";

            public static string Add(MediaType? type) => $"WatchingList/Add/{type}";

            public static string Remove(MediaType? type, string CollectionId, string TmdbId) => $"WatchingList/Remove/{type}/{CollectionId}/{TmdbId}";

            public static string Sync(MediaType? type) => $"WatchingList/Sync/{type}";
        }

        public async Task<WatchingList?> Get(bool IsUserAuthenticated)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync<WatchingList>(Endpoint.Get, false);
            }
            else
            {
                return default;
            }
        }

        public async Task<WatchingList?> Add(MediaType? mediaType, WatchingListItem? item)
        {
            if (mediaType == null) throw new ArgumentNullException(nameof(mediaType));
            if (item == null) throw new ArgumentNullException(nameof(item));

            return await PostAsync<WatchingListItem, WatchingList>(Endpoint.Add(mediaType), false, item, Endpoint.Get);
        }

        public async Task<WatchingList?> Remove(MediaType? mediaType, string? CollectionId, string? TmdbId = "null")
        {
            if (mediaType == null) throw new ArgumentNullException(nameof(mediaType));
            if (CollectionId == null) throw new ArgumentNullException(nameof(CollectionId));

            return await PostAsync<WatchingList>(Endpoint.Remove(mediaType, CollectionId, TmdbId ?? "null"), false, null, Endpoint.Get);
        }

        public async Task<WatchingList?> Sync(MediaType? mediaType)
        {
            if (mediaType == null) throw new ArgumentNullException(nameof(mediaType));

            return await PostAsync<WatchingList>(Endpoint.Sync(mediaType), false, null);
        }
    }
}