using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using SD.API.Core.Scraping;
using SD.Shared.Models.List;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.News;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;
using System.Globalization;

namespace SD.API.Functions
{
    public class CacheFunction(CosmosCacheRepository cacheRepo, IConfiguration configuration)
    {
        [Function("Settings")]
        public async Task<HttpResponseData> Configurations(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/settings")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var settings = new Settings
                {
                    ShowAdSense = configuration.GetValue<bool>("Settings:ShowAdSense"),
                };

                return await req.CreateResponse(settings, ttlCache.one_day, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("CacheNew")]
        public async Task<HttpResponseData?> CacheNew(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/cache/news")] HttpRequestData req, CancellationToken cancellationToken)
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

                        foreach (var item in obj?.Items?.Take(8) ?? [])
                        {
                            if (item == null) continue;
                            compactModels.Items.Add(new Shared.Models.News.Item(item.id, item.title, item.url_img, item.link));
                        }

                        doc = await cacheRepo.UpsertItemAsync(new NewsCache(compactModels, "lastnews_compact"), cancellationToken);
                    }
                    else
                    {
                        var fullModels = new NewsModel();

                        foreach (var item in obj?.Items ?? [])
                        {
                            if (item == null) continue;
                            fullModels.Items.Add(new Shared.Models.News.Item(item.id, item.title, item.url_img, item.link));
                        }

                        doc = await cacheRepo.UpsertItemAsync(new NewsCache(fullModels, "lastnews_full"), cancellationToken);
                    }
                }

                return await req.CreateResponse(doc, ttlCache.six_hours, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                else
                    req.ProcessException(new NotificationException("Timeout occurred"));

                return req.CreateResponse(System.Net.HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Function("CacheTrailers")]
        public async Task<HttpResponseData?> CacheTrailers(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/cache/trailers")] HttpRequestData req, CancellationToken cancellationToken)
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

                        foreach (var item in obj?.contents?.Take(10).Select(s => s.video) ?? [])
                        {
                            if (item == null) continue;
                            compactModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title, item.thumbnails[0].url));
                        }

                        doc = await cacheRepo.UpsertItemAsync(new YoutubeCache(compactModels, "lasttrailers_compact"), cancellationToken);
                    }
                    else
                    {
                        var fullModels = new TrailerModel();

                        foreach (var item in obj?.contents?.Select(s => s.video) ?? [])
                        {
                            if (item == null) continue;
                            fullModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title, item.thumbnails[2].url));
                        }

                        doc = await cacheRepo.UpsertItemAsync(new YoutubeCache(fullModels, "lasttrailers_full"), cancellationToken);
                    }
                }

                return await req.CreateResponse(doc, ttlCache.six_hours, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                else
                    req.ProcessException(new NotificationException("Timeout occurred"));

                return req.CreateResponse(System.Net.HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Function("ImdbPopularMovies")]
        public async Task<HttpResponseData?> ImdbPopularMovies(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/cache/imdb-popular-movies")] HttpRequestData req, CancellationToken cancellationToken)
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
                        var compactModels = new MostPopularData() { ErrorMessage = obj.ErrorMessage };

                        foreach (var item in obj?.Items.Take(20) ?? [])
                        {
                            if (item == null) continue;
                            compactModels.Items.Add(item);
                        }

                        doc = await cacheRepo.UpsertItemAsync(new MostPopularDataCache(compactModels, "popularmovies_compact"), cancellationToken);
                    }
                    else
                    {
                        var fullModels = new MostPopularData() { ErrorMessage = obj.ErrorMessage };

                        foreach (var item in obj?.Items ?? [])
                        {
                            if (item == null) continue;
                            fullModels.Items.Add(item);
                        }

                        doc = await cacheRepo.UpsertItemAsync(new MostPopularDataCache(fullModels, "popularmovies_full"), cancellationToken);
                    }
                }

                return await req.CreateResponse(doc, ttlCache.six_hours, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                else
                    req.ProcessException(new NotificationException("Timeout occurred"));

                return req.CreateResponse(System.Net.HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Function("ImdbPopularTVs")]
        public async Task<HttpResponseData?> ImdbPopularTVs(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/cache/imdb-popular-tvs")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var doc = await cacheRepo.Get<MostPopularData>("populartvs", cancellationToken);

                if (doc == null)
                {
                    var scraping = new ScrapingPopular();
                    var obj = scraping.GetTvData();
                    if (obj == null) return null;

                    doc = await cacheRepo.UpsertItemAsync(new MostPopularDataCache(obj, "populartvs"), cancellationToken);
                }

                return await req.CreateResponse(doc, ttlCache.six_hours, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                else
                    req.ProcessException(new NotificationException("Timeout occurred"));

                return req.CreateResponse(System.Net.HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Function("CacheMovieRatings")]
        public async Task<HttpResponseData?> CacheMovieRatings(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/cache/ratings/movie")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];
                var tmdb_id = req.GetQueryParameters()["tmdb_id"];
                var tmdb_rating = req.GetQueryParameters()["tmdb_rating"];
                var title = req.GetQueryParameters()["title"];

                DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime release_date);

                var doc = await cacheRepo.Get<Ratings>($"rating_{(id.NotEmpty() ? id : tmdb_id)}", cancellationToken);
                var ttl = ttlCache.one_day;

                if (release_date > DateTime.Now.AddDays(-7)) return null; //dont get ratings for new releases (one week)

                if (doc == null)
                {
                    var objRatings = await ApiStartup.HttpClient.GetFilmShowRatings<RatingApiRoot>(id, cancellationToken);

                    var scraping = new ScrapingRatings(req.FunctionContext.GetLogger(req.FunctionContext.FunctionDefinition.Name), objRatings);
                    var obj = scraping.GetMovieData(id, tmdb_rating, title, release_date.Year.ToString());
                    if (obj == null) return null;

                    if (release_date > DateTime.Now.AddDays(-7)) // < 1 week launch
                    {
                        ttl = ttlCache.one_day;
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    {
                        ttl = ttlCache.one_week;
                    }
                    else // > 1 month launch
                    {
                        ttl = ttlCache.three_months;
                    }

                    doc = await cacheRepo.UpsertItemAsync(new RatingsCache((id.NotEmpty() ? id : tmdb_id), obj, ttl), cancellationToken);
                }

                //add on sd certified list
                if (doc?.Data != null && release_date < DateTime.Now.AddDays(-30)) // at least 1 month launch
                {
                    var rating = doc.Data;

                    var imdb_ok = float.TryParse(rating.imdb?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float imdb);
                    var tmdb_ok = float.TryParse(rating.tmdb?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float tmdb);
                    var meta_ok = float.TryParse(rating.metacritic?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float meta);
                    var trac_ok = float.TryParse(rating.trakt?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float trac);
                    var roto_ok = float.TryParse(rating.rottenTomatoes?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float roto);
                    var fiaf_ok = float.TryParse(rating.filmAffinity?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float fiaf);

                    var count = 0;
                    if (imdb_ok && imdb >= 8) count++;
                    if (tmdb_ok && tmdb >= 8) count++;
                    if (meta_ok && meta >= 8) count++;
                    if (trac_ok && trac >= 80) count++;
                    if (roto_ok && roto >= 80) count++;
                    if (fiaf_ok && fiaf >= 8) count++;

                    if (count >= 5) //if there is at least 5 green ratings
                    {
                        var tmdb_write_token = configuration.GetValue<string>("TMDB:WriteToken");
                        await ApiStartup.HttpClient.AddTmdbListItem(8498673, int.Parse(tmdb_id!), MediaType.movie, tmdb_write_token, cancellationToken);
                    }
                }

                return await req.CreateResponse(doc, ttl, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                else
                    req.ProcessException(new NotificationException("Timeout occurred"));

                return req.CreateResponse(System.Net.HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Function("CacheShowRatings")]
        public async Task<HttpResponseData?> CacheShowRatings(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/cache/ratings/show")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];
                var tmdb_id = req.GetQueryParameters()["tmdb_id"];
                var tmdb_rating = req.GetQueryParameters()["tmdb_rating"];
                var title = req.GetQueryParameters()["title"];

                DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime release_date);

                var doc = await cacheRepo.Get<Ratings>($"rating_{(id.NotEmpty() ? id : tmdb_id)}", cancellationToken);
                var ttl = ttlCache.one_day;

                if (release_date > DateTime.Now.AddDays(-7)) return null; //dont get ratings for new releases (one week)

                if (doc == null)
                {
                    var objRatings = await ApiStartup.HttpClient.GetFilmShowRatings<RatingApiRoot>(id, cancellationToken);

                    var scraping = new ScrapingRatings(req.FunctionContext.GetLogger(req.FunctionContext.FunctionDefinition.Name), objRatings);
                    var obj = scraping.GetShowData(id, tmdb_rating, title, release_date.Year.ToString());
                    if (obj == null) return null;

                    if (release_date > DateTime.Now.AddDays(-7)) // < 1 week launch
                    {
                        ttl = ttlCache.one_day;
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    {
                        ttl = ttlCache.one_week;
                    }
                    else // > 1 month launch
                    {
                        ttl = ttlCache.three_months;
                    }

                    doc = await cacheRepo.UpsertItemAsync(new RatingsCache((id.NotEmpty() ? id : tmdb_id), obj, ttl), cancellationToken);
                }

                //add on sd certified list
                if (doc?.Data != null && release_date < DateTime.Now.AddDays(-30)) // at least 1 month launch
                {
                    var rating = doc.Data;

                    var imdb_ok = float.TryParse(rating.imdb?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float imdb);
                    var tmdb_ok = float.TryParse(rating.tmdb?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float tmdb);
                    var meta_ok = float.TryParse(rating.metacritic?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float meta);
                    var trac_ok = float.TryParse(rating.trakt?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float trac);
                    var roto_ok = float.TryParse(rating.rottenTomatoes?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float roto);
                    var fiaf_ok = float.TryParse(rating.filmAffinity?.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out float fiaf);

                    var count = 0;
                    if (imdb_ok && imdb >= 8) count++;
                    if (tmdb_ok && tmdb >= 8) count++;
                    if (meta_ok && meta >= 8) count++;
                    if (trac_ok && trac >= 80) count++;
                    if (roto_ok && roto >= 80) count++;
                    if (fiaf_ok && fiaf >= 8) count++;

                    if (count >= 5) //if there is at least 5 green ratings
                    {
                        var tmdb_write_token = configuration.GetValue<string>("TMDB:WriteToken");
                        await ApiStartup.HttpClient.AddTmdbListItem(8498675, int.Parse(tmdb_id!), MediaType.tv, tmdb_write_token, cancellationToken);
                    }
                }

                return await req.CreateResponse(doc, ttl, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                else
                    req.ProcessException(new NotificationException("Timeout occurred"));

                return req.CreateResponse(System.Net.HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Function("CacheMovieReviews")]
        public async Task<HttpResponseData?> CacheMovieReviews(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/cache/reviews/movies")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];
                DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime release_date);
                var doc = await cacheRepo.Get<ReviewModel>($"review_{id}", cancellationToken);
                var ttl = ttlCache.one_day;

                if (release_date > DateTime.Now.AddDays(-7)) return null; //dont get reviews for new releases (one week)

                if (doc == null)
                {
                    var obj = await ApiStartup.HttpClient.GetReviewsByImdb8<RootMetacritic>(id, cancellationToken);
                    if (obj == null) return null;

                    var newModel = new ReviewModel();

                    foreach (var item in obj.data?.title?.metacritic?.reviews?.edges ?? [])
                    {
                        if (item == null) continue;
                        newModel.Items.Add(new Shared.Models.Reviews.Item(item.node?.site, item.node?.url, item.node?.reviewer, item.node?.score, item.node?.quote?.value));
                    }

                    if (release_date > DateTime.Now.AddDays(-7)) // < 1 week launch
                    {
                        ttl = ttlCache.one_day;
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    {
                        ttl = ttlCache.one_week;
                    }
                    else // > 1 month launch
                    {
                        ttl = ttlCache.three_months;
                    }

                    doc = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_day), cancellationToken);
                }

                return await req.CreateResponse(doc, ttl, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                else
                    req.ProcessException(new NotificationException("Timeout occurred"));

                return req.CreateResponse(System.Net.HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        [Function("CacheShowReviews")]
        public async Task<HttpResponseData?> CacheShowReviews(
          [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/cache/reviews/shows")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];
                var title = req.GetQueryParameters()["title"];
                DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime release_date);
                var doc = await cacheRepo.Get<ReviewModel>($"review_{id}", cancellationToken);
                var ttl = ttlCache.one_day;

                if (release_date > DateTime.Now.AddDays(-7)) return null; //dont get reviews for new releases (one week)

                if (doc == null)
                {
                    var scraping = new ScrapingReview();
                    var obj = scraping.GetTvReviews(title, release_date.Year);
                    if (obj == null) return null;
                    //if (obj.meta?.title == "undefined critic reviews") return null;

                    var newModel = new ReviewModel();

                    foreach (var item in obj.items ?? [])
                    {
                        if (item == null) continue;
                        newModel.Items.Add(new Shared.Models.Reviews.Item(item.publicationName, item.url, item.author, item.score, item.quote));
                    }

                    if (release_date > DateTime.Now.AddDays(-7)) // < 1 week launch
                    {
                        ttl = ttlCache.one_day;
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    {
                        ttl = ttlCache.one_week;
                    }
                    else // > 1 month launch
                    {
                        ttl = ttlCache.three_months;
                    }

                    doc = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_day), cancellationToken);
                }

                return await req.CreateResponse(doc, ttl, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                else
                    req.ProcessException(new NotificationException("Timeout occurred"));

                return req.CreateResponse(System.Net.HttpStatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                return req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}