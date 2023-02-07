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
            public const string Ratings = "Public/Cache/Ratings";

            public static string GetRatings(string id) => $"Public/Cache/Ratings?id={id}";
        }

        public async Task<FlixsterCache?> GetNews()
        {
            return await GetAsync<FlixsterCache>(Endpoint.News, false);
        }

        public async Task<FlixsterCache?> AddNews(FlixsterCache? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.News, false, obj, Endpoint.News);
        }

        public async Task<RatingsCache?> GetRatings(string id)
        {
            return await GetAsync<RatingsCache>(Endpoint.GetRatings(id), false);
        }

        public async Task<RatingsCache?> AddRatings(RatingsCache? obj, string id)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.Ratings, false, obj, Endpoint.GetRatings(id));
        }

        public async Task<YoutubeCache?> GetTrailers()
        {
            return await GetAsync<YoutubeCache>(Endpoint.Trailers, false);
        }

        public async Task<YoutubeCache?> AddTrailers(YoutubeCache? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.Trailers, false, obj, Endpoint.Trailers);
        }
    }
}