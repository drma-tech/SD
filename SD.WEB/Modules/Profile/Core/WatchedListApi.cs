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
            public const string Post = "WatchedList/Post";
        }

        public async Task<WatchedList?> Get()
        {
            if (ComponenteUtils.IsAuthenticated)
            {
                return await GetAsync<WatchedList>(Endpoint.Get, false);
            }
            else
            {
                return new();
            }
        }

        public async Task<WatchedList?> Post(WatchedList? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.Post, false, obj, Endpoint.Get);
        }
    }
}