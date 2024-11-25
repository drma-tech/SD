using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using SD.API.Core;
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
        //[OpenApiOperation("CacheNew", "Rapid API (json)", Description = "flixster / cached - one_day")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CacheDocument<NewsModel>))]
        [Function("CacheNew")]
        public async Task<NewsModel?> CacheNew(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/News")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var mode = req.GetQueryParameters()["mode"];
                var model = await cacheRepo.Get<NewsModel>($"lastnews_{mode}", cancellationToken);

                if (model == null)
                {
                    var obj = await ApiStartup.HttpClient.GetNewsByFlixter<Flixster>(cancellationToken);
                    if (obj == null) return null;

                    if (mode == "compact")
                    {
                        var compactModels = new NewsModel();

                        foreach (var item in obj.data?.newsStories.Take(8) ?? [])
                        {
                            if (item == null) continue;
                            compactModels.Items.Add(new Shared.Models.News.Item(item.id, item.title, item.mainImage?.url, item.link));
                        }

                        var compactResult = await cacheRepo.UpsertItemAsync(new FlixsterCache(compactModels, "lastnews_compact"), cancellationToken);

                        return compactResult?.Data;
                    }
                    else
                    {
                        var fullModels = new NewsModel();

                        foreach (var item in obj.data?.newsStories ?? Enumerable.Empty<NewsStory>())
                        {
                            if (item == null) continue;
                            fullModels.Items.Add(new Shared.Models.News.Item(item.id, item.title, item.mainImage?.url, item.link));
                        }

                        var fullResult = await cacheRepo.UpsertItemAsync(new FlixsterCache(fullModels, "lastnews_full"), cancellationToken);

                        return fullResult?.Data;
                    }
                }
                else
                {
                    return model.Data;
                }
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                    return default;
                }
                else
                {
                    req.ProcessException(new NotificationException("Timeout occurred"));
                    throw;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        //[OpenApiOperation("CacheTrailers", "Rapid API (json)", Description = "youtube-search-and-download / cached - one_day")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CacheDocument<TrailerModel>))]
        [Function("CacheTrailers")]
        public async Task<TrailerModel?> CacheTrailers(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Trailers")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var mode = req.GetQueryParameters()["mode"];
                var model = await cacheRepo.Get<TrailerModel>($"lasttrailers_{mode}", cancellationToken);

                if (model == null)
                {
                    var obj = await ApiStartup.HttpClient.GetTrailersByYoutubeSearch<Youtube>(cancellationToken);
                    if (obj == null) return null;

                    if (mode == "compact")
                    {
                        var compactModels = new TrailerModel();

                        foreach (var item in obj.contents.Take(8).Select(s => s.video))
                        {
                            if (item == null) continue;
                            compactModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title, item.thumbnails[0].url));
                        }

                        var compactResult = await cacheRepo.UpsertItemAsync(new YoutubeCache(compactModels, "lasttrailers_compact"), cancellationToken);

                        return compactResult?.Data;
                    }
                    else
                    {
                        var fullModels = new TrailerModel();

                        foreach (var item in obj.contents.Select(s => s.video))
                        {
                            if (item == null) continue;
                            fullModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title, item.thumbnails[2].url));
                        }

                        var fullResult = await cacheRepo.UpsertItemAsync(new YoutubeCache(fullModels, "lasttrailers_full"), cancellationToken);

                        return fullResult?.Data;
                    }
                }
                else
                {
                    return model.Data;
                }
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                    return default;
                }
                else
                {
                    req.ProcessException(new NotificationException("Timeout occurred"));
                    throw;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        //[OpenApiOperation("ImdbPopularMovies", "IMDB (scraping)", Description = "scraping / cached - one_day")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CacheDocument<MostPopularData>))]
        [Function("ImdbPopularMovies")]
        public async Task<MostPopularData?> ImdbPopularMovies(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/ImdbPopularMovies")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var mode = req.GetQueryParameters()["mode"];
                var model = await cacheRepo.Get<MostPopularData>($"popularmovies_{mode}", cancellationToken);

                if (model == null)
                {
                    var scraping = new ScrapingPopular();
                    var obj = scraping.GetMovieData();
                    if (obj == null) return null;

                    if (mode == "compact")
                    {
                        var compactModels = new MostPopularData() { ErrorMessage = obj.ErrorMessage };

                        foreach (var item in obj.Items.Take(10))
                        {
                            if (item == null) continue;
                            compactModels.Items.Add(item);
                        }

                        var compactResult = await cacheRepo.UpsertItemAsync(new MostPopularDataCache(compactModels, "popularmovies_compact"), cancellationToken);

                        return compactResult?.Data;
                    }
                    else
                    {
                        var fullModels = new MostPopularData() { ErrorMessage = obj.ErrorMessage };

                        foreach (var item in obj.Items)
                        {
                            if (item == null) continue;
                            fullModels.Items.Add(item);
                        }

                        var fullResult = await cacheRepo.UpsertItemAsync(new MostPopularDataCache(fullModels, "popularmovies_full"), cancellationToken);

                        return fullResult?.Data;
                    }
                }
                else
                {
                    return model.Data;
                }
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                    return default;
                }
                else
                {
                    req.ProcessException(new NotificationException("Timeout occurred"));
                    throw;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        //[OpenApiOperation("ImdbPopularTVs", "IMDB (scraping)", Description = "scraping / cached - one_day")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CacheDocument<MostPopularData>))]
        [Function("ImdbPopularTVs")]
        public async Task<MostPopularData?> ImdbPopularTVs(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/ImdbPopularTVs")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var model = await cacheRepo.Get<MostPopularData>("populartvs", cancellationToken);

                if (model == null)
                {
                    var scraping = new ScrapingPopular();
                    var obj = scraping.GetTvData();
                    if (obj == null) return null;

                    model = await cacheRepo.UpsertItemAsync(new MostPopularDataCache(obj, "populartvs"), cancellationToken);
                }

                return model?.Data;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                    return default;
                }
                else
                {
                    req.ProcessException(new NotificationException("Timeout occurred"));
                    throw;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        //[OpenApiOperation("CacheMovieRatings", "Metacritic (scraping)", Description = "scraping / cached - one_day x one_year")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CacheDocument<Ratings>))]
        [Function("CacheMovieRatings")]
        public async Task<Ratings?> CacheMovieRatings(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Ratings/Movie")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                CacheDocument<Ratings>? model;

                var id = req.GetQueryParameters()["id"];
                var tmdb_id = req.GetQueryParameters()["tmdb_id"];
                var tmdb_rating = req.GetQueryParameters()["tmdb_rating"];
                var title = req.GetQueryParameters()["title"];

                DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime release_date);

                if (id.Empty()) throw new NotificationException($"id null ({title} - {release_date.Year})");

                model = await cacheRepo.Get<Ratings>($"rating_{id}", cancellationToken);

                if (model == null)
                {
                    var scraping = new ScrapingRatings();
                    var obj = scraping.GetMovieData(id, tmdb_rating, title, release_date.Year.ToString());
                    if (obj == null) return null;

                    if (release_date.Date == DateTime.MinValue.Date || release_date.Date == DateTime.MaxValue.Date) //invalid date
                    {
                        model = await cacheRepo.UpsertItemAsync(new RatingsCache(id, obj, ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddDays(-7)) // < 1 week launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new RatingsCache(id, obj, ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new RatingsCache(id, obj, ttlCache.one_week), cancellationToken);
                    }
                    else // > 1 month launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new RatingsCache(id, obj, ttlCache.one_month), cancellationToken);
                    }
                }

                if (model?.Data != null)
                {
                    var rating = model.Data;

                    var imdb_ok = float.TryParse(rating.imdb, NumberStyles.Any, CultureInfo.InvariantCulture, out float imdb);
                    var tmdb_ok = float.TryParse(rating.tmdb, NumberStyles.Any, CultureInfo.InvariantCulture, out float tmdb);
                    var meta_ok = float.TryParse(rating.metacritic, NumberStyles.Any, CultureInfo.InvariantCulture, out float meta);
                    var trac_ok = float.TryParse(rating.trakt, NumberStyles.Any, CultureInfo.InvariantCulture, out float trac);

                    if (imdb_ok && tmdb_ok && meta_ok && trac_ok)
                    {
                        if (imdb >= 8 && tmdb >= 8 && meta >= 8 && trac >= 80)
                        {
                            var tmdb_write_token = configuration.GetValue<string>("tmdb_write_token");
                            await ApiStartup.HttpClient.AddTmdbListItem(8498673, int.Parse(tmdb_id!), MediaType.movie, tmdb_write_token, cancellationToken);
                        }
                    }
                }

                return model?.Data;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                    return default;
                }
                else
                {
                    req.ProcessException(new NotificationException("Timeout occurred"));
                    throw;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        //[OpenApiOperation("CacheShowRatings", "Metacritic (scraping)", Description = "scraping / cached - one_day x one_year")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CacheDocument<Ratings>))]
        [Function("CacheShowRatings")]
        public async Task<Ratings?> CacheShowRatings(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Ratings/Show")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                CacheDocument<Ratings>? model;

                var id = req.GetQueryParameters()["id"];
                var tmdb_id = req.GetQueryParameters()["tmdb_id"];
                var tmdb_rating = req.GetQueryParameters()["tmdb_rating"];
                var title = req.GetQueryParameters()["title"];

                DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime release_date);

                if (id.Empty()) throw new NotificationException($"id null ({title} - {release_date.Year})");

                model = await cacheRepo.Get<Ratings>($"rating_{id}", cancellationToken);

                if (model == null)
                {
                    var scraping = new ScrapingRatings();
                    var obj = scraping.GetShowData(id, tmdb_rating, title, release_date.Year.ToString());
                    if (obj == null) return null;

                    if (release_date.Date == DateTime.MinValue.Date || release_date.Date == DateTime.MaxValue.Date) //invalid date
                    {
                        model = await cacheRepo.UpsertItemAsync(new RatingsCache(id, obj, ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddDays(-7)) // < 1 week launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new RatingsCache(id, obj, ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new RatingsCache(id, obj, ttlCache.one_week), cancellationToken);
                    }
                    else // > 1 month launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new RatingsCache(id, obj, ttlCache.one_month), cancellationToken);
                    }
                }

                if (model?.Data != null)
                {
                    var rating = model.Data;

                    var imdb_ok = float.TryParse(rating.imdb, NumberStyles.Any, CultureInfo.InvariantCulture, out float imdb);
                    var tmdb_ok = float.TryParse(rating.tmdb, NumberStyles.Any, CultureInfo.InvariantCulture, out float tmdb);
                    var meta_ok = float.TryParse(rating.metacritic, NumberStyles.Any, CultureInfo.InvariantCulture, out float meta);
                    var trac_ok = float.TryParse(rating.trakt, NumberStyles.Any, CultureInfo.InvariantCulture, out float trac);

                    if (imdb_ok && tmdb_ok && meta_ok && trac_ok)
                    {
                        if (imdb >= 8 && tmdb >= 8 && meta >= 8 && trac >= 80)
                        {
                            var tmdb_write_token = configuration.GetValue<string>("tmdb_write_token");
                            await ApiStartup.HttpClient.AddTmdbListItem(8498675, int.Parse(tmdb_id!), MediaType.tv, tmdb_write_token, cancellationToken);
                        }
                    }
                }

                return model?.Data;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                    return default;
                }
                else
                {
                    req.ProcessException(new NotificationException("Timeout occurred"));
                    throw;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        //[OpenApiOperation("CacheMovieReviews", "Rapid API (json)", Description = "imdb8 / cached - one_day x one_year")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CacheDocument<ReviewModel>))]
        [Function("CacheMovieReviews")]
        public async Task<ReviewModel?> CacheMovieReviews(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Reviews/Movies")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                CacheDocument<ReviewModel>? model;

                var id = req.GetQueryParameters()["id"];
                DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime release_date);
                model = await cacheRepo.Get<ReviewModel>($"review_{id}", cancellationToken);

                if (model == null)
                {
                    var obj = await ApiStartup.HttpClient.GetReviewsByImdb8<RootMetacritic>(id, cancellationToken);
                    if (obj == null) return null;

                    var newModel = new ReviewModel();

                    foreach (var item in obj.data?.title?.metacritic?.reviews?.edges ?? [])
                    {
                        if (item == null) continue;
                        newModel.Items.Add(new Shared.Models.Reviews.Item(item.node?.site, item.node?.url, item.node?.reviewer, item.node?.score, item.node?.quote?.value));
                    }

                    if (release_date.Date == DateTime.MinValue.Date || release_date.Date == DateTime.MaxValue.Date) //invalid date
                    {
                        model = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddDays(-7)) // < 1 week launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_week), cancellationToken);
                    }
                    else // > 1 month launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_month), cancellationToken);
                    }
                }

                return model?.Data;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                    return default;
                }
                else
                {
                    req.ProcessException(new NotificationException("Timeout occurred"));
                    throw;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        //[OpenApiOperation("CacheMovieReviews", "Metacritic (scraping)", Description = "scraping / cached - one_day x one_year")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(CacheDocument<ReviewModel>))]
        [Function("CacheShowReviews")]
        public async Task<ReviewModel?> CacheShowReviews(
          [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Reviews/Shows")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                //apis found that site metacritic is using
                //https://fandom-prod.apigee.net/v1/xapi/shows/metacritic/game-of-thrones/web?apiKey=1MOZgmNFxvmljaQR1X9KAij9Mo4xAY3u
                //https://fandom-prod.apigee.net/v1/xapi/reviews/metacritic/critic/shows/game-of-thrones/web?apiKey=1MOZgmNFxvmljaQR1X9KAij9Mo4xAY3u

                CacheDocument<ReviewModel>? model;

                var id = req.GetQueryParameters()["id"];
                var title = req.GetQueryParameters()["title"];
                DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime release_date);
                model = await cacheRepo.Get<ReviewModel>($"review_{id}", cancellationToken);

                return null; //todo: solve this anyway

                if (model == null)
                {
                    var url = $"https://internal-prod.apigee.fandom.net/v1/xapi/composer/metacritic/pages/shows-critic-reviews/{title}/web?filter=all&sort=score&apiKey=1MOZgmNFxvmljaQR1X9KAij9Mo4xAY3u";
                    var obj = await ApiStartup.HttpClient.Get<MetaCriticScraping>(url, cancellationToken);
                    if (obj == null) return null;
                    if (obj.meta?.title == "undefined critic reviews") return null;

                    var newModel = new ReviewModel();

                    foreach (var item in obj.components.Find(w => w.meta?.componentName == "critic-reviews")?.data?.items ?? [])
                    {
                        if (item == null) continue;
                        newModel.Items.Add(new Shared.Models.Reviews.Item(item.publicationName, item.url, item.author, item.score, item.quote));
                    }

                    if (release_date.Date == DateTime.MinValue.Date || release_date.Date == DateTime.MaxValue.Date) //invalid date
                    {
                        model = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddDays(-7)) // < 1 week launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1)) // < 1 month launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_week), cancellationToken);
                    }
                    else // > 1 month launch
                    {
                        model = await cacheRepo.UpsertItemAsync(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_month), cancellationToken);
                    }
                }

                return model?.Data;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    req.ProcessException(new NotificationException("Cancellation Requested"));
                    return default;
                }
                else
                {
                    req.ProcessException(new NotificationException("Timeout occurred"));
                    throw;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }
    }
}