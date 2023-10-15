using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List;
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

            public static string GetMovieRatings(string? id, string? title, DateTime? date, string? tmdb_rating) => $"Public/Cache/Ratings/Movie?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}&tmdb_rating={tmdb_rating}";

            public static string GetShowRatings(string? id, string? title, DateTime? date, string? tmdb_rating) => $"Public/Cache/Ratings/Show?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}&tmdb_rating={tmdb_rating}";

            public static string GetMovieReviews(string? id, string? title, DateTime? date) => $"Public/Cache/Reviews/Movies?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}";

            public static string GetShowReviews(string? id, string? title, DateTime? date) => $"Public/Cache/Reviews/Shows?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}";
        }

        public async Task<FlixsterCache?> GetNews(string mode)
        {
            return await GetAsync<FlixsterCache>(Endpoint.News(mode), false);
        }

        public async Task<YoutubeCache?> GetTrailers(string mode)
        {
            return await GetAsync<YoutubeCache>(Endpoint.Trailers(mode), false);
        }

        public async Task<RatingsCache?> GetMovieRatings(string? id, string? title, DateTime? releaseDate, string? tmdb_rating)
        {
            return await GetAsync<RatingsCache>(Endpoint.GetMovieRatings(id, title, releaseDate, tmdb_rating), false);
        }

        public async Task<RatingsCache?> GetShowRatings(string? id, string? title, DateTime? releaseDate, string? tmdb_rating)
        {
            return await GetAsync<RatingsCache>(Endpoint.GetShowRatings(id, title, releaseDate, tmdb_rating), false);
        }

        public async Task<MetaCriticCache?> GetMovieReviews(string? id, string? title, DateTime? releaseDate)
        {
            return await GetAsync<MetaCriticCache>(Endpoint.GetMovieReviews(id, title, releaseDate), false);
        }

        public async Task<MetaCriticCache?> GetShowReviews(string? id, string? title, DateTime? releaseDate)
        {
            return await GetAsync<MetaCriticCache>(Endpoint.GetShowReviews(id, title, releaseDate), false);
        }
    }
}