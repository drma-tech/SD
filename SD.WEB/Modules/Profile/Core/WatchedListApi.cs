using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Profile.Core
{
    public class WatchedListApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCore<WatchedList>(factory, memoryCache, "WatchedList")
    {
        private struct Endpoint
        {
            public const string Get = "watchedlist/get";

            public static string Add(MediaType? type, string TmdbId) => $"watchedlist/add/{type}/{TmdbId}";

            public static string Remove(MediaType? type, string TmdbId) => $"watchedlist/remove/{type}/{TmdbId}";
        }

        public async Task<WatchedList?> Get(bool IsUserAuthenticated, string? id = null)
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

        public async Task<WatchedList?> Add(MediaType? mediaType, string? TmdbId)
        {
            ArgumentNullException.ThrowIfNull(mediaType);
            ArgumentNullException.ThrowIfNull(TmdbId);

            return await PostAsync<WatchedList>(Endpoint.Add(mediaType, TmdbId), null);
        }

        public async Task<WatchedList?> Remove(MediaType? mediaType, string? TmdbId)
        {
            ArgumentNullException.ThrowIfNull(mediaType);
            ArgumentNullException.ThrowIfNull(TmdbId);

            return await PostAsync<WatchedList>(Endpoint.Remove(mediaType, TmdbId), null);
        }
    }
}