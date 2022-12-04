using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Profile.Core
{
    public class WatchingListApi : ApiServices
    {
        public WatchingListApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string Get = "WatchingList/Get";
            public const string Post = "WatchingList/Post";
        }

        public async Task<WatchingList?> Get(bool IsUserAuthenticated)
        {
            if (IsUserAuthenticated)
            {
                return await GetAsync<WatchingList>(Endpoint.Get, false);
            }
            else
            {
                return new();
            }
        }

        public async Task<WatchingList?> Post(WatchingList obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.Post, false, obj, Endpoint.Get);
        }
    }
}