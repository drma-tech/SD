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
            public const string GetNews = "Public/Cache/News/Get";
            public const string AddNews = "Public/Cache/News/Add";

            public static string GetRatings(string id) => $"Public/Cache/Ratings/Get?id={id}";

            public const string AddRatings = "Public/Cache/Ratings/Add";
            public const string GetTrailers = "Public/Cache/Trailers/Get";
            public const string AddTrailers = "Public/Cache/Trailers/Add";
        }

        public async Task<FlixsterCache?> GetNews()
        {
            return await GetAsync<FlixsterCache>(Endpoint.GetNews, false);
        }

        public async Task<FlixsterCache?> AddNews(FlixsterCache? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.AddNews, false, obj, Endpoint.GetNews);
        }

        public async Task<RatingsCache?> GetRatings(string id)
        {
            return await GetAsync<RatingsCache>(Endpoint.GetRatings(id), false);
        }

        public async Task<RatingsCache?> AddRatings(RatingsCache? obj, string id)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.AddRatings, false, obj, Endpoint.GetRatings(id));
        }

        public async Task<YoutubeCache?> GetTrailers()
        {
            return await GetAsync<YoutubeCache>(Endpoint.GetTrailers, false);
        }

        public async Task<YoutubeCache?> AddTrailers(YoutubeCache? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await PostAsync(Endpoint.AddTrailers, false, obj, Endpoint.GetTrailers);
        }
    }
}