using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.News;
using SD.Shared.Models.Trailers;

namespace SD.WEB.Core
{
    public class CacheApi : ApiServices
    {
        public CacheApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string News = "Public/Cache/News";
            public const string Trailers = "Public/Cache/Trailers";

            public static string GetRatings(string id, DateTime? date) => $"Public/Cache/Ratings?id={id}&release_date={date}";
        }

        public async Task<FlixsterCache?> GetNews()
        {
            return await GetAsync<FlixsterCache>(Endpoint.News, false);
        }

        public async Task<YoutubeCache?> GetTrailers()
        {
            return await GetAsync<YoutubeCache>(Endpoint.Trailers, false);
        }

        public async Task<RatingsCache?> GetRatings(string id, DateTime? releaseDate)
        {
            return await GetAsync<RatingsCache>(Endpoint.GetRatings(id, releaseDate), false);
        }
    }
}