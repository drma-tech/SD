using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Trailers;

namespace SD.WEB.Modules.News.Core
{
    public class TrailersApi : ApiServices
    {
        public TrailersApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public struct Endpoint
        {
            public static string Get(string id) => $"https://youtube-search-and-download.p.rapidapi.com/channel?id={id}&sort=n";
        }

        public async Task<Youtube?> Get()
        {
            return await GetByRapidYoutubeApi<Youtube>(Endpoint.Get("UCzcRQ3vRNr6fJ1A9rqFn7QA"));
        }
    }
}