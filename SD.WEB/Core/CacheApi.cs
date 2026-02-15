using SD.Shared.Models.Energy;
using SD.Shared.Models.List;
using SD.Shared.Models.News;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;

namespace SD.WEB.Core;

public struct Endpoint
{
    public static string News(string mode, string category)
    {
        return $"public/cache/news?mode={mode}&category={category}";
    }

    public static string Trailers(string mode)
    {
        return $"public/cache/trailers?mode={mode}";
    }

    public static string GetMovieRatings(string? id, string? tmdbId, string? title, DateTime? date, string? tmdbRating)
    {
        return $"public/cache/ratings/movie?id={id}&tmdb_id={tmdbId}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}&tmdb_rating={tmdbRating}";
    }

    public static string GetShowRatings(string? id, string? tmdbId, string? title, DateTime? date, string? tmdbRating)
    {
        return $"public/cache/ratings/show?id={id}&tmdb_id={tmdbId}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}&tmdb_rating={tmdbRating}";
    }

    public static string GetMovieReviews(string? id, string? title, DateTime? date)
    {
        return $"public/cache/reviews/movies?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}";
    }

    public static string GetShowReviews(string? id, string? title, DateTime? date)
    {
        return $"public/cache/reviews/shows?id={id}&title={title}&release_date={date?.ToString("yyyy-MM-dd")}";
    }
}

public class CacheFlixsterApi(IHttpClientFactory http) : ApiCosmos<CacheDocument<NewsModel>>(http, ApiType.Anonymous, null)
{
    public async Task<CacheDocument<NewsModel>?> GetNews(string mode, string category)
    {
        return await GetAsync(Endpoint.News(mode, category));
    }
}

public class CacheYoutubeApi(IHttpClientFactory http) : ApiCosmos<CacheDocument<TrailerModel>>(http, ApiType.Anonymous, null)
{
    public async Task<CacheDocument<TrailerModel>?> GetTrailers(string mode)
    {
        return await GetAsync(Endpoint.Trailers(mode));
    }
}

public class CacheRatingsApi(IHttpClientFactory http) : ApiCosmos<CacheDocument<Ratings>>(http, ApiType.Anonymous, null)
{
    public async Task<CacheDocument<Ratings>?> GetMovieRatings(string? id, string? tmdbId, string? title, DateTime? releaseDate, string? tmdbRating)
    {
        return await GetAsync(Endpoint.GetMovieRatings(id, tmdbId, title, releaseDate, tmdbRating));
    }

    public async Task<CacheDocument<Ratings>?> GetShowRatings(string? id, string? tmdbId, string? title, DateTime? releaseDate, string? tmdbRating)
    {
        return await GetAsync(Endpoint.GetShowRatings(id, tmdbId, title, releaseDate, tmdbRating));
    }
}

public class CacheMetaCriticApi(IHttpClientFactory http) : ApiCosmos<CacheDocument<ReviewModel>>(http, ApiType.Anonymous, null)
{
    public async Task<CacheDocument<ReviewModel>?> GetMovieReviews(string? id, string? title, DateTime? releaseDate)
    {
        return await GetAsync(Endpoint.GetMovieReviews(id, title, releaseDate));
    }

    public async Task<CacheDocument<ReviewModel>?> GetShowReviews(string? id, string? title, DateTime? releaseDate)
    {
        return await GetAsync(Endpoint.GetShowReviews(id, title, releaseDate));
    }
}