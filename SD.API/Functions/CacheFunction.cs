using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Caching.Distributed;
using SD.API.Core.Scraping;
using SD.Shared.Models.List;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.News;
using SD.Shared.Models.Popular;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SD.API.Functions;

public partial class CacheFunction(CosmosCacheRepository cacheRepo, IDistributedCache cache, IHttpClientFactory factory)
{
    [Function("CacheNews")]
    public async Task<HttpResponseData?> CacheNews([HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/news")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        var mode = req.GetQueryParameters()["mode"];
        var category = req.GetQueryParameters()["category"];
        var cacheKey = $"news_{mode}_{category}";

        var doc = await cache.Get<NewsCache>(cacheKey, cancellationToken);

        if (doc == null)
        {
            doc = await cacheRepo.ReadItemAsync<NewsCache>(new CacheIdentity(cacheKey), cancellationToken);

            if (doc == null)
            {
                var client = factory.CreateClient("rapidapi");
                var obj = await client.GetNewsByImdb8<NewsJson>(category, cancellationToken);

                var compactModels = new NewsModel();

                var nodes = obj?.data?.news?.edges?.Select(s => s.node) ?? [];

                foreach (var item in nodes.Take(string.Equals(mode, "compact", StringComparison.OrdinalIgnoreCase) ? 10 : 30) ?? [])
                {
                    if (item == null) continue;
                    compactModels.Items.Add(new NewsModelItem(item.id,
                        item.articleTitle?.plainText,
                        item.image?.url?.Replace("@._V1_.jpg", "@._V1_UY500_.jpg", StringComparison.OrdinalIgnoreCase), //force height to 500px
                        item.externalUrl,
                        item.date));
                }

                doc = await cacheRepo.CreateItemAsync(new NewsCache(cacheKey, compactModels));
            }

            await SaveCache(doc, cacheKey, TtlCache.HalfDay, cancellationToken);
        }

        return await req.CreateResponse(doc, TtlCache.HalfDay, cancellationToken);
    }

    [GeneratedRegex("^(?:10[1-9]|1[1-9]\\d|[2-9]\\d{2,})K\\b|^\\d+(?:\\.\\d+)?M\\b", RegexOptions.Compiled, 1000)]
    private static partial Regex IsPopular();

    [Function("CacheTrailers")]
    public async Task<HttpResponseData?> CacheTrailers(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/trailers")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var mode = req.GetQueryParameters()["mode"];
        var cacheKey = $"trailers_{mode}";

        var doc = await cache.Get<YoutubeCache>(cacheKey, cancellationToken);

        if (doc == null)
        {
            doc = await cacheRepo.ReadItemAsync<YoutubeCache>(new CacheIdentity(cacheKey), cancellationToken);

            if (doc == null)
            {
                var client = factory.CreateClient("rapidapi");
                var obj = await client.GetTrailersByYoutubeSearch<Youtube>(cancellationToken);

                var compactModels = new TrailerModel();

                foreach (var item in obj?.contents?.Take(string.Equals(mode, "compact", StringComparison.OrdinalIgnoreCase) ? 12 : 100).Select(s => s.video) ?? [])
                {
                    if (item == null) continue;
                    compactModels.Items.Add(new TrailerModelItem(item.videoId, item.title,
                        string.Equals(mode, "compact", StringComparison.OrdinalIgnoreCase) ? item.thumbnails[1].url : item.thumbnails[2].url, item.publishedTimeText, item.publishedTimeText.ParseRelativeDate(),
                        IsPopular().IsMatch(item.viewCountText ?? "")));
                }

                doc = await cacheRepo.CreateItemAsync(new YoutubeCache(cacheKey, compactModels));
            }

            await SaveCache(doc, cacheKey, TtlCache.SixHours, cancellationToken);
        }

        return await req.CreateResponse(doc, TtlCache.SixHours, cancellationToken);
    }

    [Function("ImdbPopularMovies")]
    public async Task<HttpResponseData?> ImdbPopularMovies(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/imdb-popular-movies")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var mode = req.GetQueryParameters()["mode"];
        var cacheKey = $"popular-movies-{mode}";

        var doc = await cache.Get<MostPopularDataCache>(cacheKey, cancellationToken);

        if (doc == null)
        {
            doc = await cacheRepo.ReadItemAsync<MostPopularDataCache>(new CacheIdentity(cacheKey), cancellationToken);

            if (doc == null)
            {
                var client = factory.CreateClient("rapidapi");
                var obj = await client.GetMostPopular<List<PopularScraping>>("most-popular-movies", cancellationToken);

                var compactModels = new MostPopularData();

                foreach (var item in obj?.Take(string.Equals(mode, "compact", StringComparison.OrdinalIgnoreCase) ? 20 : 50) ?? [])
                {
                    if (item == null) continue;

                    var image = item.thumbnails != null && item.thumbnails.Length > 1 ? item.thumbnails[1].url : null;

                    compactModels.Items.Add(new MostPopularDataDetail
                    {
                        Id = item.id,
                        Title = item.primaryTitle,
                        Image = image?.Replace("@._V1_QL75_UX280_CR0,0,280,414_.jpg", "@._V1_QL75_UX130_.jpg", StringComparison.OrdinalIgnoreCase),
                        Year = item.startYear?.ToString(CultureInfo.InvariantCulture),
                        IMDbRating = item.averageRating?.ToString("0.0", CultureInfo.InvariantCulture),
                    });
                }

                doc = await cacheRepo.CreateItemAsync(new MostPopularDataCache(cacheKey, compactModels));
            }

            await SaveCache(doc, cacheKey, TtlCache.TwoDays, cancellationToken);
        }

        return await req.CreateResponse(doc, TtlCache.TwoDays, cancellationToken);
    }

    [Function("ImdbPopularTVs")]
    public async Task<HttpResponseData?> ImdbPopularTVs(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/imdb-popular-tv")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var mode = req.GetQueryParameters()["mode"];
        var cacheKey = $"popular-tv-{mode}";

        var doc = await cache.Get<MostPopularDataCache>(cacheKey, cancellationToken);

        if (doc == null)
        {
            doc = await cacheRepo.ReadItemAsync<MostPopularDataCache>(new CacheIdentity(cacheKey), cancellationToken);

            if (doc == null)
            {
                var client = factory.CreateClient("rapidapi");
                var obj = await client.GetMostPopular<List<PopularScraping>>("most-popular-tv", cancellationToken);

                var compactModels = new MostPopularData();

                foreach (var item in obj?.Take(string.Equals(mode, "compact", StringComparison.OrdinalIgnoreCase) ? 20 : 50) ?? [])
                {
                    if (item == null) continue;

                    var image = item.thumbnails != null && item.thumbnails.Length > 1 ? item.thumbnails[1].url : null;

                    compactModels.Items.Add(new MostPopularDataDetail
                    {
                        Id = item.id,
                        Title = item.primaryTitle,
                        Image = image,
                        Year = item.startYear?.ToString(CultureInfo.InvariantCulture),
                        IMDbRating = item.averageRating?.ToString("0.0", CultureInfo.InvariantCulture),
                    });
                }

                doc = await cacheRepo.CreateItemAsync(new MostPopularDataCache(cacheKey, compactModels));
            }

            await SaveCache(doc, cacheKey, TtlCache.TwoDays, cancellationToken);
        }

        return await req.CreateResponse(doc, TtlCache.TwoDays, cancellationToken);
    }

    [Function("CacheMovieRatings")]
    public async Task<HttpResponseData?> CacheMovieRatings(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/ratings/movie")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var id = req.GetQueryParameters()["id"];
        var tmdbId = req.GetQueryParameters()["tmdb_id"];
        var tmdbRating = req.GetQueryParameters()["tmdb_rating"];
        var ttl = TtlCache.OneDay;

        DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate);
        var cacheKey = $"rating_{(id.NotEmpty() ? id : tmdbId)}";

        var doc = await cache.Get<RatingsCache>(cacheKey, cancellationToken);

        if (doc == null)
        {
            if (releaseDate > DateTime.Now.AddDays(-7)) return null; //don't get ratings for new releases (first week of launch)

            doc = await cacheRepo.ReadItemAsync<RatingsCache>(new CacheIdentity(cacheKey), cancellationToken);

            if (doc == null)
            {
                var ratings = new Ratings()
                {
                    imdbId = id,
                    tmdbId = tmdbId,
                    type = MediaType.movie,
                    tmdb = tmdbRating,
                };

                //todo: do scrap only with the right url (no guess anymore)

                await cache.ExecuteWithCooldownAsync("filmshow", () => req.ProcessApiFilmShowRatings(factory, ratings, cancellationToken), cancellationToken);

                await cache.ExecuteWithCooldownAsync("unifiedmovie", () => req.ProcessApiUnifiedMovie(factory, ratings, cancellationToken), cancellationToken);

                //https://rapidapi.com/jpbermoy/api/movie-database-api1 rotten tomatoes

                await cache.ExecuteWithCooldownAsync("moviesratings2", () => req.ProcessApiMoviesRatings2(factory, ratings, cancellationToken), cancellationToken);

                ttl = CalculateTtl(releaseDate);

                doc = await cacheRepo.CreateItemAsync(new RatingsCache(id.NotEmpty() ? id : tmdbId!, ratings, ttl));
            }

            await SaveCache(doc, cacheKey, ttl, cancellationToken);
        }

        await TrySaveCertifiedSd(doc, releaseDate, 8498673, tmdbId, MediaType.movie, factory, cancellationToken);

        return await req.CreateResponse(doc, ttl, cancellationToken);
    }

    [Function("CacheShowRatings")]
    public async Task<HttpResponseData?> CacheShowRatings(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/ratings/show")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var id = req.GetQueryParameters()["id"];
        var tmdbId = req.GetQueryParameters()["tmdb_id"] ?? throw new NotificationException("tmdb_id is required");
        var tmdbRating = req.GetQueryParameters()["tmdb_rating"];
        var ttl = TtlCache.OneDay;

        DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate);
        var cacheKey = $"rating_{(id.NotEmpty() ? id : tmdbId)}";

        var doc = await cache.Get<RatingsCache>(cacheKey, cancellationToken);

        if (doc == null)
        {
            if (releaseDate > DateTime.Now.AddDays(-7)) return null; //don't get ratings for new releases (first week of launch)

            doc = await cacheRepo.ReadItemAsync<RatingsCache>(new CacheIdentity(cacheKey), cancellationToken);

            if (doc == null)
            {
                var ratings = new Ratings()
                {
                    imdbId = id,
                    tmdbId = tmdbId,
                    type = MediaType.tv,
                    tmdb = tmdbRating,
                };

                await cache.ExecuteWithCooldownAsync("filmshow", () => req.ProcessApiFilmShowRatings(factory, ratings, cancellationToken), cancellationToken);

                await cache.ExecuteWithCooldownAsync("unifiedmovie", () => req.ProcessApiUnifiedMovie(factory, ratings, cancellationToken), cancellationToken);

                //https://rapidapi.com/jpbermoy/api/movie-database-api1 rotten tomatoes

                await cache.ExecuteWithCooldownAsync("moviesratings2", () => req.ProcessApiMoviesRatings2(factory, ratings, cancellationToken), cancellationToken);

                ttl = CalculateTtl(releaseDate);

                doc = await cacheRepo.CreateItemAsync(new RatingsCache(id.NotEmpty() ? id : tmdbId, ratings, ttl));
            }

            await SaveCache(doc, cacheKey, ttl, cancellationToken);
        }

        await TrySaveCertifiedSd(doc, releaseDate, 8498675, tmdbId, MediaType.tv, factory, cancellationToken);

        return await req.CreateResponse(doc, ttl, cancellationToken);
    }

    [Function("CacheMovieReviews")]
    public async Task<HttpResponseData?> CacheMovieReviews(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/reviews/movies")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var id = req.GetQueryParameters()["id"];
        var ttl = TtlCache.OneWeek;

        DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate);
        var cacheKey = $"review_{id}";

        var doc = await cache.Get<MetaCriticCache>(cacheKey, cancellationToken);

        if (doc == null)
        {
            if (releaseDate > DateTime.Now.AddDays(-14)) return null; //don't get reviews for new releases (first two weeks of launch)

            doc = await cacheRepo.ReadItemAsync<MetaCriticCache>(new CacheIdentity(cacheKey), cancellationToken);

            if (doc == null)
            {
                var client = factory.CreateClient("rapidapi");
                var obj = await client.GetReviewsByImdb8<RootMetacritic>(id, cancellationToken);
                if (obj == null) return null;

                var newModel = new ReviewModel();

                foreach (var node in obj.data?.title?.metacritic?.reviews?.edges.Select(s => s.node) ?? [])
                {
                    newModel.Items.Add(new ReviewModelItem(node?.site, node?.url, node?.reviewer, node?.score, node?.quote?.value));
                }

                ttl = CalculateTtl(releaseDate);

                doc = await cacheRepo.CreateItemAsync(new MetaCriticCache($"review_{id}", newModel, ttl));
            }

            await SaveCache(doc, cacheKey, ttl, cancellationToken);
        }

        return await req.CreateResponse(doc, ttl, cancellationToken);
    }

    private static async Task TrySaveCertifiedSd(CacheDocumentData<Ratings>? doc, DateTime releaseDate, int listId, string? tmdbId, MediaType type, IHttpClientFactory factory, CancellationToken token)
    {
        if (tmdbId.Empty()) return;

        if (doc?.Data != null && releaseDate < DateTime.Now.AddDays(-15)) // at least 2 weeks launch
        {
            var rating = doc.Data;

            var imdbOk = float.TryParse(rating.imdb?.Replace(",", ".", StringComparison.OrdinalIgnoreCase), NumberStyles.Any, CultureInfo.InvariantCulture, out var imdb);
            var tmdbOk = float.TryParse(rating.tmdb?.Replace(",", ".", StringComparison.OrdinalIgnoreCase), NumberStyles.Any, CultureInfo.InvariantCulture, out var tmdb);
            var metaOk = float.TryParse(rating.metacritic?.Replace(",", ".", StringComparison.OrdinalIgnoreCase), NumberStyles.Any, CultureInfo.InvariantCulture, out var meta);
            var tracOk = float.TryParse(rating.trakt?.Replace(",", ".", StringComparison.OrdinalIgnoreCase), NumberStyles.Any, CultureInfo.InvariantCulture, out var trac);
            var rotoOk = float.TryParse(rating.rottenTomatoes?.Replace(",", ".", StringComparison.OrdinalIgnoreCase), NumberStyles.Any, CultureInfo.InvariantCulture, out var roto);
            var fiafOk = float.TryParse(rating.filmAffinity?.Replace(",", ".", StringComparison.OrdinalIgnoreCase), NumberStyles.Any, CultureInfo.InvariantCulture, out var fiaf);
            var lettOk = float.TryParse(rating.letterboxd?.Replace(",", ".", StringComparison.OrdinalIgnoreCase), NumberStyles.Any, CultureInfo.InvariantCulture, out var lett);

            var count = 0;
            var value = 7.95f;

            if (imdbOk && imdb >= value) count++;
            if (tmdbOk && tmdb >= value) count++;
            if (metaOk && meta >= value) count++;
            if (tracOk && trac >= value && trac <= 10) count++; //new scale 0-10
            if (rotoOk && roto >= value && roto <= 10) count++; //new scale 0-10
            if (fiafOk && fiaf >= value) count++;
            if (lettOk && lett >= value) count++; //scale changed to 0-10

            if (count >= 4) //if there is at least 4 green ratings
            {
                var tmdbWriteToken = ApiStartup.Configurations.TMDB?.WriteToken;
                var client = factory.CreateClient("tmdb");
                try
                {
                    await client.AddTmdbListItem(listId, int.Parse(tmdbId, CultureInfo.InvariantCulture), type, tmdbWriteToken, token);
                }
                catch (Exception)
                {
                    //if the item is already in the list
                }
            }
        }
    }

    private static TtlCache CalculateTtl(DateTime releaseDate)
    {
        if (releaseDate > DateTime.Now.AddDays(-7)) // 1 week launch or future releases
        {
            return TtlCache.HalfWeek;
        }

        if (releaseDate > DateTime.Now.AddDays(-30)) // less than 1 month launch
        {
            return TtlCache.TwoWeeks;
        }

        if (releaseDate > DateTime.Now.AddDays(-60)) // less than 2 months launch
        {
            return TtlCache.OneMonth;
        }

        return TtlCache.SixMonths; // older then one month
    }

    private async Task SaveCache<TData>(CacheDocumentData<TData>? doc, string cacheKey, TtlCache ttl, CancellationToken cancellationToken) where TData : class, new()
    {
        if (doc != null)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(doc);
            await cache.SetAsync(cacheKey, bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds((int)ttl) }, cancellationToken);
        }
    }
}