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

public class CacheFlixsterApi(IHttpClientFactory http) : ApiCosmos<NewsCache>(http, ApiType.Anonymous, null, [], ApiContext.Default.NewsCache)
{
    public async Task<NewsCache?> GetNews(string mode, string category, ComponentActions<NewsCache> actions, CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.News(mode, category), false, actions, cancellationToken);
    }
}

public class CacheYoutubeApi(IHttpClientFactory http) : ApiCosmos<YoutubeCache>(http, ApiType.Anonymous, null, [], ApiContext.Default.YoutubeCache)
{
    public async Task<YoutubeCache?> GetTrailers(string mode, ComponentActions<YoutubeCache> actions, CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.Trailers(mode), false, actions, cancellationToken);
    }
}

public class CacheRatingsApi(IHttpClientFactory http) : ApiCosmos<RatingsCache>(http, ApiType.Anonymous, null, [], ApiContext.Default.RatingsCache)
{
    public async Task<RatingsCache?> GetMovieRatings(string? id, string? tmdbId, string? title, DateTime? releaseDate, string? tmdbRating, ComponentActions<RatingsCache> actions, CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.GetMovieRatings(id, tmdbId, title, releaseDate, tmdbRating), false, actions, cancellationToken);
    }

    public async Task<RatingsCache?> GetShowRatings(string? id, string? tmdbId, string? title, DateTime? releaseDate, string? tmdbRating, ComponentActions<RatingsCache> actions, CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.GetShowRatings(id, tmdbId, title, releaseDate, tmdbRating), false, actions, cancellationToken);
    }
}

public class CacheMetaCriticApi(IHttpClientFactory http) : ApiCosmos<MetaCriticCache>(http, ApiType.Anonymous, null, [], ApiContext.Default.MetaCriticCache)
{
    public async Task<MetaCriticCache?> GetMovieReviews(string? id, string? title, DateTime? releaseDate, ComponentActions<MetaCriticCache> actions, CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.GetMovieReviews(id, title, releaseDate), false, actions, cancellationToken);
    }

    public async Task<MetaCriticCache?> GetShowReviews(string? id, string? title, DateTime? releaseDate, ComponentActions<MetaCriticCache> actions, CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.GetShowReviews(id, title, releaseDate), false, actions, cancellationToken);
    }
}