using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.News;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;

namespace SD.WEB.Core
{
    public class CacheApi : ApiServices
    {
        public CacheApi(IHttpClientFactory http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public static string News(string mode) => $"Public/Cache/News?mode={mode}";

            public static string Trailers(string mode) => $"Public/Cache/Trailers?mode={mode}";

            public static string GetMovieReviews(string id, DateTime? date) => $"Public/Cache/Reviews/Movies?id={id}&release_date={date}";

            public static string GetShowReviews(string id, string title, DateTime? date) => $"Public/Cache/Reviews/Shows?id={id}&title={title}&release_date={date}";
        }

        public async Task<FlixsterCache?> GetNews(string mode)
        {
            return await GetAsync<FlixsterCache>(Endpoint.News(mode), false);
        }

        public async Task<YoutubeCache?> GetTrailers(string mode)
        {
            return await GetAsync<YoutubeCache>(Endpoint.Trailers(mode), false);
        }

        public async Task<MetaCriticCache?> GetMovieReviews(string id, DateTime? releaseDate)
        {
            return await GetAsync<MetaCriticCache>(Endpoint.GetMovieReviews(id, releaseDate), false);
        }

        public async Task<MetaCriticCache?> GetShowReviews(string id, string title, DateTime? releaseDate)
        {
            return await GetAsync<MetaCriticCache>(Endpoint.GetShowReviews(id, title, releaseDate), false);
        }
    }
}