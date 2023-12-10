using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Profile.Core
{
    public class WatchedListApi(IHttpClientFactory http, IMemoryCache memoryCache) : ApiCore<WatchedList>(http, memoryCache, "WatchedList")
    {
        private struct Endpoint
        {
            public const string Get = "watchedlist/get";

            public static string Add(MediaType? type, string TmdbId) => $"watchedlist/add/{type}/{TmdbId}";

            public static string Remove(MediaType? type, string TmdbId) => $"watchedlist/remove/{type}/{TmdbId}";
        }

        public async Task<WatchedList?> Get(string? id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                return await GetAsync($"{Endpoint.Get}?id={id}");
            }
            else
            {
                return await GetAsync(Endpoint.Get);
            }
        }

        public async Task<WatchedList?> Add(MediaType? mediaType, string? TmdbId)
        {
            ArgumentNullException.ThrowIfNull(mediaType);
            ArgumentNullException.ThrowIfNull(TmdbId);

            return await PostAsync<WatchedList>(Endpoint.Add(mediaType, TmdbId), null);
        }

        public async Task<WatchedList?> Remove(MediaType? mediaType, string? TmdbId)
        {
            if (mediaType == null) throw new ArgumentNullException(nameof(mediaType));
            if (TmdbId == null) throw new ArgumentNullException(nameof(TmdbId));

            return await PostAsync<WatchedList>(Endpoint.Remove(mediaType, TmdbId), null);
        }
    }
}