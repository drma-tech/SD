using SD.Shared.Models.List;
using SD.Shared.Models.News;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;
using SD.WEB.Shared;

namespace SD.WEB.Core
{
    public struct Endpoint
    {
        public static string News(string mode) => $"public/cache/news?mode={mode}";

        public static string Trailers(string mode) => $"public/cache/trailers?mode={mode}";

        public static string GetMovieRatings(string? id, string? tmdb_id, string? title, DateTime? date, string? tmdb_rating) => $"public/cache/ratings/movie?id={id}&tmdb_id={tmdb_id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}&tmdb_rating={tmdb_rating}";

        public static string GetShowRatings(string? id, string? tmdb_id, string? title, DateTime? date, string? tmdb_rating) => $"public/cache/ratings/show?id={id}&tmdb_id={tmdb_id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}&tmdb_rating={tmdb_rating}";

        public static string GetMovieReviews(string? id, string? title, DateTime? date) => $"public/cache/reviews/movies?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}";

        public static string GetShowReviews(string? id, string? title, DateTime? date) => $"public/cache/reviews/shows?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}";
    }

    public class CacheFlixsterApi(IHttpClientFactory http) : ApiCosmos<CacheDocument<NewsModel>>(http)
    {
        public async Task<CacheDocument<NewsModel>?> GetNews(string mode, RenderControlCore<CacheDocument<NewsModel>?>? core)
        {
            return await GetAsync(Endpoint.News(mode), core);
        }
    }

    public class CacheYoutubeApi(IHttpClientFactory http) : ApiCosmos<CacheDocument<TrailerModel>>(http)
    {
        public async Task<CacheDocument<TrailerModel>?> GetTrailers(string mode, RenderControlCore<CacheDocument<TrailerModel>?>? core)
        {
            return await GetAsync(Endpoint.Trailers(mode), core);
        }
    }

    public class CacheRatingsApi(IHttpClientFactory http) : ApiCosmos<CacheDocument<Ratings>>(http)
    {
        public async Task<CacheDocument<Ratings>?> GetMovieRatings(string? id, string? tmdb_id, string? title, DateTime? releaseDate, string? tmdb_rating, RenderControlCore<CacheDocument<Ratings>?>? core)
        {
            return await GetAsync(Endpoint.GetMovieRatings(id, tmdb_id, title, releaseDate, tmdb_rating), core);
        }

        public async Task<CacheDocument<Ratings>?> GetShowRatings(string? id, string? tmdb_id, string? title, DateTime? releaseDate, string? tmdb_rating, RenderControlCore<CacheDocument<Ratings>?>? core)
        {
            return await GetAsync(Endpoint.GetShowRatings(id, tmdb_id, title, releaseDate, tmdb_rating), core);
        }
    }

    public class CacheMetaCriticApi(IHttpClientFactory http) : ApiCosmos<CacheDocument<ReviewModel>>(http)
    {
        public async Task<CacheDocument<ReviewModel>?> GetMovieReviews(string? id, string? title, DateTime? releaseDate, RenderControlCore<CacheDocument<ReviewModel>?>? core)
        {
            return await GetAsync(Endpoint.GetMovieReviews(id, title, releaseDate), core);
        }

        public async Task<CacheDocument<ReviewModel>?> GetShowReviews(string? id, string? title, DateTime? releaseDate, RenderControlCore<CacheDocument<ReviewModel>?>? core)
        {
            return await GetAsync(Endpoint.GetShowReviews(id, title, releaseDate), core);
        }
    }
}