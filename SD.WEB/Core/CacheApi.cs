using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List;
using SD.Shared.Models.News;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;

namespace SD.WEB.Core
{
    public struct Endpoint
    {
        public static string News(string mode) => $"Public/Cache/News?mode={mode}";

        public static string Trailers(string mode) => $"Public/Cache/Trailers?mode={mode}";

        public static string GetMovieRatings(string? id, string? title, DateTime? date, string? tmdb_rating) => $"Public/Cache/Ratings/Movie?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}&tmdb_rating={tmdb_rating}";

        public static string GetShowRatings(string? id, string? title, DateTime? date, string? tmdb_rating) => $"Public/Cache/Ratings/Show?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}&tmdb_rating={tmdb_rating}";

        public static string GetMovieReviews(string? id, string? title, DateTime? date) => $"Public/Cache/Reviews/Movies?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}";

        public static string GetShowReviews(string? id, string? title, DateTime? date) => $"Public/Cache/Reviews/Shows?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}";
    }

    public class CacheFlixsterApi(IHttpClientFactory http, IMemoryCache memoryCache) : ApiCore<NewsModel?>(http, memoryCache, "NewsModel")
    {
        public async Task<NewsModel?> GetNews(string mode)
        {
            return await GetAsync(Endpoint.News(mode), $"NewsModel-{mode}");
        }
    }

    public class CacheYoutubeApi(IHttpClientFactory http, IMemoryCache memoryCache) : ApiCore<TrailerModel?>(http, memoryCache, "TrailerModel")
    {
        public async Task<TrailerModel?> GetTrailers(string mode)
        {
            return await GetAsync(Endpoint.Trailers(mode), $"TrailerModel-{mode}");
        }
    }

    public class CacheRatingsApi(IHttpClientFactory http, IMemoryCache memoryCache) : ApiCore<Ratings?>(http, memoryCache, "Ratings")
    {
        public async Task<Ratings?> GetMovieRatings(string? id, string? title, DateTime? releaseDate, string? tmdb_rating)
        {
            return await GetAsync(Endpoint.GetMovieRatings(id, title, releaseDate, tmdb_rating));
        }

        public async Task<Ratings?> GetShowRatings(string? id, string? title, DateTime? releaseDate, string? tmdb_rating)
        {
            return await GetAsync(Endpoint.GetShowRatings(id, title, releaseDate, tmdb_rating));
        }
    }

    public class CacheMetaCriticApi(IHttpClientFactory http, IMemoryCache memoryCache) : ApiCore<ReviewModel?>(http, memoryCache, "ReviewModel")
    {
        public async Task<ReviewModel?> GetMovieReviews(string? id, string? title, DateTime? releaseDate)
        {
            return await GetAsync(Endpoint.GetMovieReviews(id, title, releaseDate));
        }

        public async Task<ReviewModel?> GetShowReviews(string? id, string? title, DateTime? releaseDate)
        {
            return await GetAsync(Endpoint.GetShowReviews(id, title, releaseDate));
        }
    }
}