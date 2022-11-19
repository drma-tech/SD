using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models;

namespace SD.WEB.Core
{
    public class CacheApi : ApiServices
    {
        public CacheApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string Get = "Cache/Get?key={0}";
            public const string Add = "Cache/Add";
        }

        public async Task<CacheModel?> Get(string key)
        {
            var url = Endpoint.Get.Format(key);
            return await GetAsync<CacheModel>(url, false);
        }

        public async Task<CacheModel?> Add(CacheModel? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.Add, false, obj, Endpoint.Get);
        }
    }
}