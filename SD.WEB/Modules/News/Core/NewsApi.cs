using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.News;

namespace SD.WEB.Modules.News.Core
{
    public class NewsApi : ApiServices
    {
        public NewsApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public struct Endpoint
        {
            public const string Get = "https://flixster.p.rapidapi.com/news/list";
        }

        public async Task<Flixster?> Get()
        {
            return await GetByRapidApi<Flixster>(Endpoint.Get);
        }
    }
}