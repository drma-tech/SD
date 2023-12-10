using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Profile.Core
{
    public class WatchingListApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCore<WatchingList>(factory, memoryCache, "WatchingList")
    {
        private struct Endpoint
        {
            public const string Get = "watchinglist/get";

            public static string Add(MediaType? type) => $"watchinglist/add/{type}";

            public static string Remove(MediaType? type, string CollectionId, string TmdbId) => $"watchinglist/remove/{type}/{CollectionId}/{TmdbId}";

            public static string Sync(MediaType? type) => $"watchinglist/sync/{type}";
        }

        public async Task<WatchingList?> Get(bool IsUserAuthenticated, string? id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return await GetAsync($"{Endpoint.Get}?id={id}");
            }
            else if (IsUserAuthenticated)
            {
                return await GetAsync(Endpoint.Get);
            }
            else
            {
                return new();
            }
        }

        public async Task<WatchingList?> Add(MediaType? mediaType, WatchingListItem? item)
        {
            ArgumentNullException.ThrowIfNull(mediaType);
            ArgumentNullException.ThrowIfNull(item);

            return await PostAsync<WatchingListItem>(Endpoint.Add(mediaType), item);
        }

        public async Task<WatchingList?> Remove(MediaType? mediaType, string? CollectionId, string? TmdbId = "null")
        {
            ArgumentNullException.ThrowIfNull(mediaType);
            ArgumentNullException.ThrowIfNull(CollectionId);

            return await PostAsync<WatchingList>(Endpoint.Remove(mediaType, CollectionId, TmdbId ?? "null"), null);
        }

        public async Task<WatchingList?> Sync(MediaType? mediaType)
        {
            ArgumentNullException.ThrowIfNull(mediaType);

            return await PostAsync<WatchingList>(Endpoint.Sync(mediaType), null);
        }
    }
}