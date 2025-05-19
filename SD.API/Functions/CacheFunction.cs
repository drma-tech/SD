using System.Globalization;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using SD.API.Core.Scraping;
using SD.Shared.Models.List;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.News;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;
using Item = SD.Shared.Models.News.Item;

namespace SD.API.Functions;

public class CacheFunction(CosmosCacheRepository cacheRepo, IConfiguration configuration)
{
    [Function("Settings")]
    public async Task<HttpResponseData> Configurations(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/settings")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var settings = new Settings
            {
                ShowAdSense = configuration.GetValue<bool>("Settings:ShowAdSense")
            };

            return await req.CreateResponse(settings, TtlCache.OneDay, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("CacheNew")]
    public async Task<HttpResponseData?> CacheNew(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/news")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var mode = req.GetQueryParameters()["mode"];
            var doc = await cacheRepo.Get<NewsModel>($"lastnews_{mode}", cancellationToken);

            if (doc == null)
            {
                var scraping = new ScrapingNews();
                var obj = scraping.GetNews();

                if (mode == "compact")
                {
                    var compactModels = new NewsModel();

                    foreach (var item in obj.Items.Take(10))
                    {
                        compactModels.Items.Add(new Item(item.id, item.title, item.url_img, item.link));
                    }

                    doc = await cacheRepo.UpsertItemAsync(new NewsCache(compactModels, "lastnews_compact"),
                        cancellationToken);
                }
                else
                {
                    var fullModels = new NewsModel();

                    foreach (var item in obj.Items)
                    {
                        fullModels.Items.Add(new Item(item.id, item.title, item.url_img, item.link));
                    }

                    doc = await cacheRepo.UpsertItemAsync(new NewsCache(fullModels, "lastnews_full"),
                        cancellationToken);
                }
            }

            return await req.CreateResponse(doc, TtlCache.SixHours, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.ProcessException(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            return await req.CreateResponse<CacheDocument<NewsModel>>(null, TtlCache.SixHours, cancellationToken);
        }
    }

    [Function("CacheTrailers")]
    public async Task<HttpResponseData?> CacheTrailers(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/trailers")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var mode = req.GetQueryParameters()["mode"];
            var doc = await cacheRepo.Get<TrailerModel>($"lasttrailers_{mode}", cancellationToken);

            if (doc == null)
            {
                var obj = await ApiStartup.HttpClient.GetTrailersByYoutubeSearch<Youtube>(cancellationToken);

                if (mode == "compact")
                {
                    var compactModels = new TrailerModel();

                    foreach (var item in obj?.contents?.Take(12).Select(s => s.video) ?? [])
                    {
                        if (item == null) continue;
                        compactModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title,
                            item.thumbnails[0].url));
                    }

                    doc = await cacheRepo.UpsertItemAsync(new YoutubeCache(compactModels, "lasttrailers_compact"),
                        cancellationToken);
                }
                else
                {
                    var fullModels = new TrailerModel();

                    foreach (var item in obj?.contents?.Select(s => s.video) ?? [])
                    {
                        if (item == null) continue;
                        fullModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title,
                            item.thumbnails[2].url));
                    }

                    doc = await cacheRepo.UpsertItemAsync(new YoutubeCache(fullModels, "lasttrailers_full"),
                        cancellationToken);
                }
            }

            return await req.CreateResponse(doc, TtlCache.SixHours, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.ProcessException(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            return await req.CreateResponse<CacheDocument<TrailerModel>>(null, TtlCache.SixHours, cancellationToken);
        }
    }

    [Function("ImdbPopularMovies")]
    public async Task<HttpResponseData?> ImdbPopularMovies(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/imdb-popular-movies")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var mode = req.GetQueryParameters()["mode"];
            var doc = await cacheRepo.Get<MostPopularData>($"popularmovies_{mode}", cancellationToken);

            if (doc == null)
            {
                var scraping = new ScrapingPopular();
                var obj = scraping.GetMovieData();

                if (mode == "compact")
                {
                    var compactModels = new MostPopularData { ErrorMessage = obj.ErrorMessage };

                    foreach (var item in obj.Items.Take(20))
                    {
                        compactModels.Items.Add(item);
                    }

                    doc = await cacheRepo.UpsertItemAsync(
                        new MostPopularDataCache(compactModels, "popularmovies_compact"), cancellationToken);
                }
                else
                {
                    var fullModels = new MostPopularData { ErrorMessage = obj.ErrorMessage };

                    foreach (var item in obj.Items)
                    {
                        fullModels.Items.Add(item);
                    }

                    doc = await cacheRepo.UpsertItemAsync(new MostPopularDataCache(fullModels, "popularmovies_full"),
                        cancellationToken);
                }
            }

            return await req.CreateResponse(doc, TtlCache.SixHours, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.ProcessException(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            return await req.CreateResponse<CacheDocument<MostPopularData>>(null, TtlCache.SixHours,
                cancellationToken);
        }
    }

    [Function("ImdbPopularTVs")]
    public async Task<HttpResponseData?> ImdbPopularTVs(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/imdb-popular-tvs")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var doc = await cacheRepo.Get<MostPopularData>("populartvs", cancellationToken);

            if (doc == null)
            {
                var scraping = new ScrapingPopular();
                var obj = scraping.GetTvData();

                doc = await cacheRepo.UpsertItemAsync(new MostPopularDataCache(obj, "populartvs"), cancellationToken);
            }

            return await req.CreateResponse(doc, TtlCache.SixHours, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.ProcessException(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            return await req.CreateResponse<CacheDocument<MostPopularData>>(null, TtlCache.SixHours,
                cancellationToken);
        }
    }

    [Function("CacheMovieRatings")]
    public async Task<HttpResponseData?> CacheMovieRatings(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/ratings/movie")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var id = req.GetQueryParameters()["id"];
            var tmdbId = req.GetQueryParameters()["tmdb_id"];
            var tmdbRating = req.GetQueryParameters()["tmdb_rating"];
            var title = req.GetQueryParameters()["title"];

            DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var releaseDate);

            var doc = await cacheRepo.Get<Ratings>($"rating_{(id.NotEmpty() ? id : tmdbId)}", cancellationToken);
            var ttl = TtlCache.OneDay;

            if (releaseDate > DateTime.Now.AddDays(-7)) return null; //don't get ratings for new releases (one week)

            if (doc == null)
            {
                var objRatings = await ApiStartup.HttpClient.GetFilmShowRatings<RatingApiRoot>(id, cancellationToken);

                var scraping =
                    new ScrapingRatings(req.FunctionContext.GetLogger(req.FunctionContext.FunctionDefinition.Name),
                        objRatings);
                var obj = scraping.GetMovieData(id, tmdbRating, title, releaseDate.Year.ToString());

                if (releaseDate > DateTime.Now.AddDays(-7)) // < 1 week launch
                    ttl = TtlCache.OneDay;
                else if (releaseDate > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    ttl = TtlCache.OneWeek;
                else // > 1 month launch
                    ttl = TtlCache.ThreeMonths;

                doc = await cacheRepo.UpsertItemAsync(new RatingsCache(id.NotEmpty() ? id : tmdbId, obj, ttl),
                    cancellationToken);
            }

            //add on sd certified list
            if (doc?.Data != null && releaseDate < DateTime.Now.AddDays(-30)) // at least 1 month launch
            {
                var rating = doc.Data;

                var imdbOk = float.TryParse(rating.imdb?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var imdb);
                var tmdbOk = float.TryParse(rating.tmdb?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var tmdb);
                var metaOk = float.TryParse(rating.metacritic?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var meta);
                var tracOk = float.TryParse(rating.trakt?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var trac);
                var rotoOk = float.TryParse(rating.rottenTomatoes?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var roto);
                var fiafOk = float.TryParse(rating.filmAffinity?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var fiaf);

                var count = 0;
                if (imdbOk && imdb >= 8) count++;
                if (tmdbOk && tmdb >= 8) count++;
                if (metaOk && meta >= 8) count++;
                if (tracOk && trac >= 80) count++;
                if (rotoOk && roto >= 80) count++;
                if (fiafOk && fiaf >= 8) count++;

                if (count >= 5) //if there is at least 5 green ratings
                {
                    var tmdbWriteToken = configuration.GetValue<string>("TMDB:WriteToken");
                    await ApiStartup.HttpClient.AddTmdbListItem(8498673, int.Parse(tmdbId!), MediaType.movie,
                        tmdbWriteToken, cancellationToken);
                }
            }

            return await req.CreateResponse(doc, ttl, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.ProcessException(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            return await req.CreateResponse<CacheDocument<Ratings>>(null, TtlCache.SixHours, cancellationToken);
        }
    }

    [Function("CacheShowRatings")]
    public async Task<HttpResponseData?> CacheShowRatings(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/ratings/show")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var id = req.GetQueryParameters()["id"];
            var tmdbId = req.GetQueryParameters()["tmdb_id"];
            var tmdbRating = req.GetQueryParameters()["tmdb_rating"];
            var title = req.GetQueryParameters()["title"];

            DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var releaseDate);

            var doc = await cacheRepo.Get<Ratings>($"rating_{(id.NotEmpty() ? id : tmdbId)}", cancellationToken);
            var ttl = TtlCache.OneDay;

            if (releaseDate > DateTime.Now.AddDays(-7)) return null; //don't get ratings for new releases (one week)

            if (doc == null)
            {
                var objRatings = await ApiStartup.HttpClient.GetFilmShowRatings<RatingApiRoot>(id, cancellationToken);

                var scraping =
                    new ScrapingRatings(req.FunctionContext.GetLogger(req.FunctionContext.FunctionDefinition.Name),
                        objRatings);
                var obj = scraping.GetShowData(id, tmdbRating, title, releaseDate.Year.ToString());

                if (releaseDate > DateTime.Now.AddDays(-7)) // < 1 week launch
                    ttl = TtlCache.OneDay;
                else if (releaseDate > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    ttl = TtlCache.OneWeek;
                else // > 1 month launch
                    ttl = TtlCache.ThreeMonths;

                doc = await cacheRepo.UpsertItemAsync(new RatingsCache(id.NotEmpty() ? id : tmdbId, obj, ttl),
                    cancellationToken);
            }

            //add on sd certified list
            if (doc?.Data != null && releaseDate < DateTime.Now.AddDays(-30)) // at least 1 month launch
            {
                var rating = doc.Data;

                var imdbOk = float.TryParse(rating.imdb?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var imdb);
                var tmdbOk = float.TryParse(rating.tmdb?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var tmdb);
                var metaOk = float.TryParse(rating.metacritic?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var meta);
                var tracOk = float.TryParse(rating.trakt?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var trac);
                var rotoOk = float.TryParse(rating.rottenTomatoes?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var roto);
                var fiafOk = float.TryParse(rating.filmAffinity?.Replace(",", "."), NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var fiaf);

                var count = 0;
                if (imdbOk && imdb >= 8) count++;
                if (tmdbOk && tmdb >= 8) count++;
                if (metaOk && meta >= 8) count++;
                if (tracOk && trac >= 80) count++;
                if (rotoOk && roto >= 80) count++;
                if (fiafOk && fiaf >= 8) count++;

                if (count >= 5) //if there is at least 5 green ratings
                {
                    var tmdbWriteToken = configuration.GetValue<string>("TMDB:WriteToken");
                    await ApiStartup.HttpClient.AddTmdbListItem(8498675, int.Parse(tmdbId!), MediaType.tv,
                        tmdbWriteToken, cancellationToken);
                }
            }

            return await req.CreateResponse(doc, ttl, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.ProcessException(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            return await req.CreateResponse<CacheDocument<Ratings>>(null, TtlCache.SixHours, cancellationToken);
        }
    }

    [Function("CacheMovieReviews")]
    public async Task<HttpResponseData?> CacheMovieReviews(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/reviews/movies")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var id = req.GetQueryParameters()["id"];
            DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var releaseDate);
            var doc = await cacheRepo.Get<ReviewModel>($"review_{id}", cancellationToken);
            var ttl = TtlCache.OneDay;

            if (releaseDate > DateTime.Now.AddDays(-7)) return null; //don't get reviews for new releases (one week)

            if (doc == null)
            {
                var obj = await ApiStartup.HttpClient.GetReviewsByImdb8<RootMetacritic>(id, cancellationToken);
                if (obj == null) return null;

                var newModel = new ReviewModel();

                foreach (var node in obj.data?.title?.metacritic?.reviews?.edges.Select(s => s.node) ?? [])
                {
                    newModel.Items.Add(new Shared.Models.Reviews.Item(node?.site, node?.url,
                        node?.reviewer, node?.score, node?.quote?.value));
                }

                if (releaseDate > DateTime.Now.AddDays(-7)) // < 1 week launch
                    ttl = TtlCache.OneDay;
                else if (releaseDate > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    ttl = TtlCache.OneWeek;
                else // > 1 month launch
                    ttl = TtlCache.ThreeMonths;

                doc = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", TtlCache.OneDay),
                    cancellationToken);
            }

            return await req.CreateResponse(doc, ttl, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.ProcessException(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            return await req.CreateResponse<CacheDocument<ReviewModel>>(null, TtlCache.SixHours, cancellationToken);
        }
    }

    [Function("CacheShowReviews")]
    public async Task<HttpResponseData?> CacheShowReviews(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/cache/reviews/shows")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var id = req.GetQueryParameters()["id"];
            var title = req.GetQueryParameters()["title"];
            DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var releaseDate);
            var doc = await cacheRepo.Get<ReviewModel>($"review_{id}", cancellationToken);
            var ttl = TtlCache.OneDay;

            if (releaseDate > DateTime.Now.AddDays(-7)) return null; //don't get reviews for new releases (one week)

            if (doc == null)
            {
                var scraping = new ScrapingReview();
                var obj = scraping.GetTvReviews(title, releaseDate.Year);
                //if (obj.meta?.title == "undefined critic reviews") return null;

                var newModel = new ReviewModel();

                foreach (var item in obj.items)
                {
                    newModel.Items.Add(new Shared.Models.Reviews.Item(item.publicationName, item.url, item.author,
                        item.score, item.quote));
                }

                if (releaseDate > DateTime.Now.AddDays(-7)) // < 1 week launch
                    ttl = TtlCache.OneDay;
                else if (releaseDate > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    ttl = TtlCache.OneWeek;
                else // > 1 month launch
                    ttl = TtlCache.ThreeMonths;

                doc = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", TtlCache.OneDay),
                    cancellationToken);
            }

            return await req.CreateResponse(doc, ttl, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.ProcessException(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            return await req.CreateResponse<CacheDocument<ReviewModel>>(null, TtlCache.SixHours, cancellationToken);
        }
    }
}