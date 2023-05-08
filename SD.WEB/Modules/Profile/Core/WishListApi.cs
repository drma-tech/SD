using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Profile.Core
{
    public class WishListApi : ApiServices
    {
        public WishListApi(IHttpClientFactory http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string Get = "WishList/Get";

            public static string Add(MediaType? type) => $"WishList/Add/{type}";

            public static string Remove(MediaType? type, string TmdbId) => $"WishList/Remove/{type}/{TmdbId}";
        }

        public async Task<WishList?> Get(bool IsUserAuthenticated)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync<WishList>(Endpoint.Get, false);
            }
            else
            {
                return default;
            }
        }

        public async Task<WishList?> Add(MediaType? mediaType, WishListItem? item)
        {
            if (mediaType == null) throw new ArgumentNullException(nameof(mediaType));
            if (item == null) throw new ArgumentNullException(nameof(item));

            return await PostAsync<WishListItem, WishList>(Endpoint.Add(mediaType), false, item, Endpoint.Get);
        }

        public async Task<WishList?> Remove(MediaType? mediaType, string? TmdbId)
        {
            if (mediaType == null) throw new ArgumentNullException(nameof(mediaType));
            if (TmdbId == null) throw new ArgumentNullException(nameof(TmdbId));

            return await PostAsync<WishList>(Endpoint.Remove(mediaType, TmdbId), false, null, Endpoint.Get);
        }
    }
}