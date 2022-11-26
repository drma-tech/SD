using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Profile.Core
{
    public class WishListApi : ApiServices
    {
        public WishListApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string Get = "WishList/Get";
            public const string Post = "WishList/Post";
        }

        public async Task<WishList?> Get()
        {
            if (ComponenteUtils.IsAuthenticated)
            {
                return await GetAsync<WishList>(Endpoint.Get, false);
            }
            else
            {
                return new();
            }
        }

        public async Task<WishList?> Post(WishList obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.Post, false, obj, Endpoint.Get);
        }
    }
}