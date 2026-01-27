using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Caching.Distributed;
using SD.API.Core.Auth;
using SD.API.Core.Scraping;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Energy;
using SD.Shared.Models.List;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.News;
using SD.Shared.Models.Popular;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;
using System.Globalization;
using System.Net;
using System.Text.Json;
using Item = SD.Shared.Models.News.Item;

namespace SD.API.Functions;

public class CacheFunction(CosmosCacheRepository cacheRepo, CosmosRepository repo, IDistributedCache distributedCache, IHttpClientFactory factory)
{
    [Function("Energy")]
    public async Task<HttpResponseData?> Energy(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/energy")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var ip = req.GetUserIP(false);
            var cacheKey = $"energy_{DateTime.UtcNow.Day}_{ip}";

            var doc = await cacheRepo.Get<EnergyModel>(cacheKey, cancellationToken);
            var model = doc?.Data;

            model ??= new EnergyModel() { ConsumedEnergy = 0, TotalEnergy = 10 };

            doc = await cacheRepo.UpsertItemAsync(new EnergyCache(model, cacheKey), cancellationToken); //check if upsert is needed
            await SaveCache(doc, cacheKey, TtlCache.OneWeek);

            return await req.CreateResponse(doc, TtlCache.OneWeek, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            return await req.CreateResponse<CacheDocument<EnergyModel>>(null, TtlCache.OneWeek, cancellationToken);
        }
    }

    [Function("EnergyAuth")]
    public async Task<HttpResponseData?> EnergyAuth(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "cache/energy")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var ip = req.GetUserIP(false);
            var userId = await req.GetUserIdAsync(cancellationToken);

            var cacheKey = $"energy_auth_{DateTime.UtcNow.Day}_{ip}";

            var doc = await cacheRepo.Get<EnergyModel>(cacheKey, cancellationToken);
            var model = doc?.Data;

            model ??= new EnergyModel() { ConsumedEnergy = 0, TotalEnergy = 10 };

            var principal = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken);

            if (principal?.GetActiveSubscription() != null)
            {
                model.TotalEnergy = principal.GetActiveSubscription()!.ActiveProduct.GetRestrictions().Energy;
            }

            doc = await cacheRepo.UpsertItemAsync(new EnergyCache(model, cacheKey), cancellationToken); //todo: check if upsert is needed
            await SaveCache(doc, cacheKey, TtlCache.OneWeek);

            return await req.CreateResponse(doc, TtlCache.OneWeek, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            return await req.CreateResponse<CacheDocument<EnergyModel>>(null, TtlCache.OneWeek, cancellationToken);
        }
    }

    [Function("EnergyAdd")]
    public async Task EnergyAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/cache/energy/add")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var ip = req.GetUserIP(false);
            var cacheKey = $"energy_{DateTime.UtcNow.Day}_{ip}";

            var doc = await cacheRepo.Get<EnergyModel>(cacheKey, cancellationToken);

            if (doc == null)
            {
                var model = new EnergyModel() { ConsumedEnergy = 1, TotalEnergy = 10 };

                doc = await cacheRepo.UpsertItemAsync(new EnergyCache(model, cacheKey), cancellationToken);
            }
            else
            {
                doc.Data!.ConsumedEnergy += 1;
            }

            await cacheRepo.UpsertItemAsync(doc!, cancellationToken);
            await SaveCache(doc, cacheKey, TtlCache.OneWeek);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
        }
    }

    [Function("EnergyAuthAdd")]
    public async Task EnergyAuthAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "cache/energy/add")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var ip = req.GetUserIP(false);
            var userId = await req.GetUserIdAsync(cancellationToken);

            var cacheKey = $"energy_auth_{DateTime.UtcNow.Day}_{ip}";
            var doc = await cacheRepo.Get<EnergyModel>(cacheKey, cancellationToken);

            if (doc == null)
            {
                var model = new EnergyModel() { ConsumedEnergy = 1, TotalEnergy = 10 };
                var principal = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken);

                if (principal?.GetActiveSubscription() != null)
                {
                    model!.TotalEnergy = principal.GetActiveSubscription()!.ActiveProduct.GetRestrictions().Energy;
                }

                doc = await cacheRepo.UpsertItemAsync(new EnergyCache(model, cacheKey), cancellationToken);
            }
            else
            {
                doc.Data!.ConsumedEnergy += 1;
            }

            await cacheRepo.UpsertItemAsync(doc!, cancellationToken);
            await SaveCache(doc, cacheKey, TtlCache.OneWeek);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
        }
    }

    [Function("CacheNew")]
    public async Task<HttpResponseData?> CacheNew([HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/news")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            req.ValidateWebVersion();

            var mode = req.GetQueryParameters()["mode"];
            var cacheKey = $"lastnews_{mode}";
            CacheDocument<NewsModel>? doc;
            var cachedBytes = await distributedCache.GetAsync(cacheKey, cancellationToken);
            if (cachedBytes is { Length: > 0 })
            {
                doc = JsonSerializer.Deserialize<CacheDocument<NewsModel>>(cachedBytes);
            }
            else
            {
                doc = await cacheRepo.Get<NewsModel>(cacheKey, cancellationToken);

                if (doc == null)
                {
                    var client = factory.CreateClient("rapidapi");
                    var obj = await client.GetNewsByImdb8<NewsJson>(cancellationToken);

                    var compactModels = new NewsModel();

                    var nodes = obj?.data?.news?.edges?.Select(s => s.node) ?? [];

                    foreach (var item in nodes.Take(mode == "compact" ? 10 : 30) ?? [])
                    {
                        if (item == null) continue;
                        compactModels.Items.Add(new Item(item.id, item.articleTitle?.plainText, item.image?.url, item.externalUrl, item.date));
                    }

                    doc = await cacheRepo.UpsertItemAsync(new NewsCache(compactModels, $"lastnews_{mode}"), cancellationToken);
                }

                await SaveCache(doc, cacheKey, TtlCache.HalfDay);
            }

            return await req.CreateResponse(doc, TtlCache.HalfDay, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("CacheTrailers")]
    public async Task<HttpResponseData?> CacheTrailers(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/trailers")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            req.ValidateWebVersion();

            var mode = req.GetQueryParameters()["mode"];
            var cacheKey = $"lasttrailers_{mode}";
            CacheDocument<TrailerModel>? doc;

            var cachedBytes = await distributedCache.GetAsync(cacheKey, cancellationToken);
            if (cachedBytes is { Length: > 0 })
            {
                doc = JsonSerializer.Deserialize<CacheDocument<TrailerModel>>(cachedBytes);
            }
            else
            {
                doc = await cacheRepo.Get<TrailerModel>(cacheKey, cancellationToken);

                if (doc == null)
                {
                    var client = factory.CreateClient("rapidapi");
                    var obj = await client.GetTrailersByYoutubeSearch<Youtube>(cancellationToken);

                    var compactModels = new TrailerModel();

                    foreach (var item in obj?.contents?.Take(mode == "compact" ? 12 : 100).Select(s => s.video) ?? [])
                    {
                        if (item == null) continue;
                        compactModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title, item.thumbnails[0].url, item.publishedTimeText));
                    }

                    doc = await cacheRepo.UpsertItemAsync(new YoutubeCache(compactModels, $"lasttrailers_{mode}"), cancellationToken);
                }

                await SaveCache(doc, cacheKey, TtlCache.SixHours);
            }

            return await req.CreateResponse(doc, TtlCache.SixHours, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("ImdbPopularMovies")]
    public async Task<HttpResponseData?> ImdbPopularMovies(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/imdb-popular-movies")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            req.ValidateWebVersion();

            var mode = req.GetQueryParameters()["mode"];
            var cacheKey = $"popularmovies_{mode}";
            CacheDocument<MostPopularData>? doc;
            var cachedBytes = await distributedCache.GetAsync(cacheKey, cancellationToken);
            if (cachedBytes is { Length: > 0 })
            {
                doc = JsonSerializer.Deserialize<CacheDocument<MostPopularData>>(cachedBytes);
            }
            else
            {
                doc = await cacheRepo.Get<MostPopularData>(cacheKey, cancellationToken);

                if (doc == null)
                {
                    var client = factory.CreateClient("rapidapi");
                    var obj = await client.GetMostPopular<List<PopularScraping>>("most-popular-movies", cancellationToken);

                    var compactModels = new MostPopularData();

                    foreach (var item in obj?.Take(mode == "compact" ? 20 : 100) ?? [])
                    {
                        if (item == null) continue;
                        compactModels.Items.Add(new MostPopularDataDetail
                        {
                            Id = item.id,
                            Title = item.primaryTitle,
                            Image = item.thumbnails?[1]?.url,
                            Year = item.startYear?.ToString(),
                            IMDbRating = item.averageRating?.ToString("0.0", CultureInfo.InvariantCulture)
                        });
                    }

                    doc = await cacheRepo.UpsertItemAsync(new MostPopularDataCache(compactModels, $"popularmovies_{mode}"), cancellationToken);
                }

                await SaveCache(doc, cacheKey, TtlCache.OneDay);
            }

            return await req.CreateResponse(doc, TtlCache.OneDay, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("ImdbPopularTVs")]
    public async Task<HttpResponseData?> ImdbPopularTVs(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/imdb-popular-tvs")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            req.ValidateWebVersion();

            var cacheKey = "populartvs";
            CacheDocument<MostPopularData>? doc;
            var cachedBytes = await distributedCache.GetAsync(cacheKey, cancellationToken);
            if (cachedBytes is { Length: > 0 })
            {
                doc = JsonSerializer.Deserialize<CacheDocument<MostPopularData>>(cachedBytes);
            }
            else
            {
                doc = await cacheRepo.Get<MostPopularData>(cacheKey, cancellationToken);

                if (doc == null)
                {
                    var client = factory.CreateClient("rapidapi");
                    var obj = await client.GetMostPopular<List<PopularScraping>>("most-popular-tv", cancellationToken);

                    var compactModels = new MostPopularData();

                    foreach (var item in obj?.Take(20) ?? [])
                    {
                        if (item == null) continue;
                        compactModels.Items.Add(new MostPopularDataDetail
                        {
                            Id = item.id,
                            Title = item.primaryTitle,
                            Image = item.thumbnails?[1]?.url,
                            Year = item.startYear?.ToString(),
                            IMDbRating = item.averageRating?.ToString("0.0", CultureInfo.InvariantCulture)
                        });
                    }

                    doc = await cacheRepo.UpsertItemAsync(new MostPopularDataCache(compactModels, $"populartvs"), cancellationToken);
                }

                await SaveCache(doc, cacheKey, TtlCache.OneDay);
            }

            return await req.CreateResponse(doc, TtlCache.OneDay, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("CacheMovieRatings")]
    public async Task<HttpResponseData?> CacheMovieRatings(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/ratings/movie")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            req.ValidateWebVersion();

            var id = req.GetQueryParameters()["id"];
            var tmdbId = req.GetQueryParameters()["tmdb_id"];
            var tmdbRating = req.GetQueryParameters()["tmdb_rating"];

            DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate);
            var cacheKey = $"rating_{(id.NotEmpty() ? id : tmdbId)}";
            CacheDocument<Ratings>? doc;
            var cachedBytes = await distributedCache.GetAsync(cacheKey, cancellationToken);
            var ttl = TtlCache.OneDay;

            if (cachedBytes is { Length: > 0 })
            {
                doc = JsonSerializer.Deserialize<CacheDocument<Ratings>>(cachedBytes);
            }
            else
            {
                if (releaseDate > DateTime.Now.AddDays(-7)) return null; //don't get ratings for new releases (first week of launch)

                doc = await cacheRepo.Get<Ratings>(cacheKey, cancellationToken);

                if (doc == null)
                {
                    var ratings = new Ratings()
                    {
                        imdbId = id,
                        tmdbId = tmdbId,
                        type = MediaType.movie,
                        tmdb = tmdbRating,
                    };

                    await req.ProcessApiFilmShowRatings(factory, ratings, cancellationToken);

                    await req.ProcessApiUnifiedMovie(factory, ratings, cancellationToken);

                    //https://rapidapi.com/jpbermoy/api/movie-database-api1 rotten tomatoes

                    await req.ProcessApiMoviesRatings2(factory, ratings, cancellationToken);

                    ttl = CalculateTtl(releaseDate);

                    doc = await cacheRepo.UpsertItemAsync(new RatingsCache(id.NotEmpty() ? id : tmdbId, ratings, ttl), cancellationToken);
                }

                await SaveCache(doc, cacheKey, ttl);
            }

            await TrySaveCertifiedSd(doc, releaseDate, 8498673, tmdbId, MediaType.movie, cancellationToken, factory);

            return await req.CreateResponse(doc, ttl, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("CacheShowRatings")]
    public async Task<HttpResponseData?> CacheShowRatings(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/ratings/show")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            req.ValidateWebVersion();

            var id = req.GetQueryParameters()["id"];
            var tmdbId = req.GetQueryParameters()["tmdb_id"];
            var tmdbRating = req.GetQueryParameters()["tmdb_rating"];

            DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate);
            var cacheKey = $"rating_{(id.NotEmpty() ? id : tmdbId)}";
            CacheDocument<Ratings>? doc;
            var cachedBytes = await distributedCache.GetAsync(cacheKey, cancellationToken);
            var ttl = TtlCache.OneDay;

            if (cachedBytes is { Length: > 0 })
            {
                doc = JsonSerializer.Deserialize<CacheDocument<Ratings>>(cachedBytes);
            }
            else
            {
                if (releaseDate > DateTime.Now.AddDays(-7)) return null; //don't get ratings for new releases (first week of launch)

                doc = await cacheRepo.Get<Ratings>(cacheKey, cancellationToken);

                if (doc == null)
                {
                    var ratings = new Ratings()
                    {
                        imdbId = id,
                        tmdbId = tmdbId,
                        type = MediaType.tv,
                        tmdb = tmdbRating,
                    };

                    await req.ProcessApiFilmShowRatings(factory, ratings, cancellationToken);

                    await req.ProcessApiUnifiedMovie(factory, ratings, cancellationToken);

                    //https://rapidapi.com/jpbermoy/api/movie-database-api1 rotten tomatoes

                    await req.ProcessApiMoviesRatings2(factory, ratings, cancellationToken);

                    ttl = CalculateTtl(releaseDate);

                    doc = await cacheRepo.UpsertItemAsync(new RatingsCache(id.NotEmpty() ? id : tmdbId, ratings, ttl), cancellationToken);
                }

                await SaveCache(doc, cacheKey, ttl);
            }

            await TrySaveCertifiedSd(doc, releaseDate, 8498675, tmdbId, MediaType.tv, cancellationToken, factory);

            return await req.CreateResponse(doc, ttl, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("CacheMovieReviews")]
    public async Task<HttpResponseData?> CacheMovieReviews(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/reviews/movies")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            req.ValidateWebVersion();

            var id = req.GetQueryParameters()["id"];
            DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate);
            var cacheKey = $"review_{id}";
            CacheDocument<ReviewModel>? doc;
            var cachedBytes = await distributedCache.GetAsync(cacheKey, cancellationToken);
            var ttl = TtlCache.OneDay;

            if (cachedBytes is { Length: > 0 })
            {
                doc = JsonSerializer.Deserialize<CacheDocument<ReviewModel>>(cachedBytes);
            }
            else
            {
                if (releaseDate > DateTime.Now.AddDays(-14)) return null; //don't get reviews for new releases (first week of launch)

                doc = await cacheRepo.Get<ReviewModel>(cacheKey, cancellationToken);

                if (doc == null)
                {
                    var client = factory.CreateClient("rapidapi");
                    var obj = await client.GetReviewsByImdb8<RootMetacritic>(id, cancellationToken);
                    if (obj == null) return null;

                    var newModel = new ReviewModel();

                    foreach (var node in obj.data?.title?.metacritic?.reviews?.edges.Select(s => s.node) ?? [])
                    {
                        newModel.Items.Add(new Shared.Models.Reviews.Item(node?.site, node?.url,
                            node?.reviewer, node?.score, node?.quote?.value));
                    }

                    ttl = CalculateTtl(releaseDate);

                    doc = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttl), cancellationToken);
                }

                await SaveCache(doc, cacheKey, ttl);
            }

            return await req.CreateResponse(doc, ttl, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("CacheShowReviews")]
    public async Task<HttpResponseData?> CacheShowReviews(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/reviews/shows")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            req.ValidateWebVersion();

            var id = req.GetQueryParameters()["id"];
            var title = req.GetQueryParameters()["title"];
            DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate);
            var cacheKey = $"review_{id}";
            CacheDocument<ReviewModel>? doc;
            var cachedBytes = await distributedCache.GetAsync(cacheKey, cancellationToken);
            var ttl = TtlCache.OneDay;

            if (cachedBytes is { Length: > 0 })
            {
                doc = JsonSerializer.Deserialize<CacheDocument<ReviewModel>>(cachedBytes);
            }
            else
            {
                if (releaseDate > DateTime.Now.AddDays(-14)) return null; //don't get reviews for new releases (first week of launch)

                doc = await cacheRepo.Get<ReviewModel>(cacheKey, cancellationToken);

                if (doc == null)
                {
                    var obj = ScrapingReview.GetTvReviews(title, releaseDate.Year);
                    //if (obj.meta?.title == "undefined critic reviews") return null;

                    var newModel = new ReviewModel();

                    foreach (var item in obj.items)
                    {
                        newModel.Items.Add(new Shared.Models.Reviews.Item(item.publicationName, item.url, item.author, item.score, item.quote));
                    }

                    ttl = CalculateTtl(releaseDate);

                    doc = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttl), cancellationToken);
                }

                await SaveCache(doc, cacheKey, ttl);
            }

            return await req.CreateResponse(doc, ttl, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    private static async Task TrySaveCertifiedSd(CacheDocument<Ratings>? doc, DateTime releaseDate, int listId, string? tmdbId, MediaType type, CancellationToken token, IHttpClientFactory factory)
    {
        if (tmdbId.Empty()) return;

        if (doc?.Data != null && releaseDate < DateTime.Now.AddDays(-30)) // at least 1 month launch
        {
            var rating = doc.Data;

            var imdbOk = float.TryParse(rating.imdb?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var imdb);
            var tmdbOk = float.TryParse(rating.tmdb?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var tmdb);
            var metaOk = float.TryParse(rating.metacritic?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var meta);
            var tracOk = float.TryParse(rating.trakt?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var trac);
            var rotoOk = float.TryParse(rating.rottenTomatoes?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var roto);
            var fiafOk = float.TryParse(rating.filmAffinity?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var fiaf);
            var lettOk = float.TryParse(rating.letterboxd?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var lett);

            var count = 0;
            if (imdbOk && imdb >= 8) count++;
            if (tmdbOk && tmdb >= 8) count++;
            if (metaOk && meta >= 8) count++;
            if (tracOk && trac >= 8 && trac <= 10) count++; //new scale 0-10
            if (rotoOk && roto >= 8 && roto <= 10) count++; //new scale 0-10
            if (fiafOk && fiaf >= 8) count++;
            if (lettOk && lett >= 8) count++; //scale changed to 0-10

            if (count >= 4) //if there is at least 4 green ratings
            {
                var tmdbWriteToken = ApiStartup.Configurations.TMDB?.WriteToken;
                var client = factory.CreateClient("tmdb");
                await client.AddTmdbListItem(listId, int.Parse(tmdbId), type, tmdbWriteToken, token);
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

    private async Task SaveCache<TData>(CacheDocument<TData>? doc, string cacheKey, TtlCache ttl) where TData : class, new()
    {
        if (doc != null)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(doc);
            await distributedCache.SetAsync(cacheKey, bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds((int)ttl) });
        }
    }
}