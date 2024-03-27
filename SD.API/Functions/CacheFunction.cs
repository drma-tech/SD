using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Scraping;
using SD.Shared.Models.List;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.News;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;
using System.Globalization;

namespace SD.API.Functions
{
    public class CacheFunction(CosmosCacheRepository cacheRepo)
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
                    using var http = new HttpClient();
                    var obj = await http.GetNewsByFlixter<Flixster>(cancellationToken);
                    if (obj == null) return null;

                    //compact

                    var compactModels = new NewsModel();

                    foreach (var item in obj.data?.newsStories.Take(8) ?? Enumerable.Empty<NewsStory>())
                    {
                        if (item == null) continue;
                        compactModels.Items.Add(new Shared.Models.News.Item(item.id, item.title, item.mainImage?.url, item.link));
                    }

                    var compactResult = await cacheRepo.Add(new FlixsterCache(compactModels, "lastnews_compact"), cancellationToken);

                    //full

                    var fullModels = new NewsModel();

                    foreach (var item in obj.data?.newsStories ?? Enumerable.Empty<NewsStory>())
                    {
                        if (item == null) continue;
                        fullModels.Items.Add(new Shared.Models.News.Item(item.id, item.title, item.mainImage?.url, item.link));
                    }

                    var fullResult = await cacheRepo.Add(new FlixsterCache(fullModels, "lastnews_full"), cancellationToken);

                    if (mode == "compact")
                        return compactResult?.Data;
                    else
                        return fullResult?.Data;
                }
                else
                {
                    return model.Data;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
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
                    using var http = new HttpClient();
                    var obj = await http.GetTrailersByYoutubeSearch<Youtube>(cancellationToken);
                    if (obj == null) return null;

                    //compact

                    var compactModels = new TrailerModel();

                    foreach (var item in obj.contents.Take(8).Select(s => s.video))
                    {
                        if (item == null) continue;
                        compactModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title, item.thumbnails[0].url));
                    }

                    var compactResult = await cacheRepo.Add(new YoutubeCache(compactModels, "lasttrailers_compact"), cancellationToken);

                    //full

                    var fullModels = new TrailerModel();

                    foreach (var item in obj.contents.Select(s => s.video))
                    {
                        if (item == null) continue;
                        fullModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title, item.thumbnails[2].url));
                    }

                    var fullResult = await cacheRepo.Add(new YoutubeCache(fullModels, "lasttrailers_full"), cancellationToken);

                    if (mode == "compact")
                        return compactResult?.Data;
                    else
                        return fullResult?.Data;
                }
                else
                {
                    return model.Data;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
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

                    //compact

                    var compactModels = new MostPopularData() { ErrorMessage = obj.ErrorMessage };

                    foreach (var item in obj.Items.Take(10))
                    {
                        if (item == null) continue;
                        compactModels.Items.Add(item);
                    }

                    var compactResult = await cacheRepo.Add(new MostPopularDataCache(compactModels, "popularmovies_compact"), cancellationToken);

                    //full

                    var fullModels = new MostPopularData() { ErrorMessage = obj.ErrorMessage };

                    foreach (var item in obj.Items)
                    {
                        if (item == null) continue;
                        fullModels.Items.Add(item);
                    }

                    var fullResult = await cacheRepo.Add(new MostPopularDataCache(fullModels, "popularmovies_full"), cancellationToken);

                    if (mode == "compact")
                        return compactResult?.Data;
                    else
                        return fullResult?.Data;
                }
                else
                {
                    return model.Data;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
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
                    using var http = new HttpClient();
                    var scraping = new ScrapingPopular();
                    var obj = scraping.GetTvData();
                    if (obj == null) return null;

                    model = await cacheRepo.Add(new MostPopularDataCache(obj, "populartvs"), cancellationToken);
                }

                return model?.Data;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
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
                var tmdb_rating = req.GetQueryParameters()["tmdb_rating"];
                var title = req.GetQueryParameters()["title"];
                DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime release_date);
                model = await cacheRepo.Get<Ratings>($"rating_new_{id}", cancellationToken);

                if (model == null)
                {
                    var scraping = new ScrapingRatings();
                    var obj = scraping.GetMovieData(id, tmdb_rating, title, release_date.Year.ToString());
                    if (obj == null) return null;

                    if (release_date.Date == DateTime.MinValue.Date || release_date.Date == DateTime.MaxValue.Date)
                    {
                        //invalid date
                        model = await cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1))
                    {
                        // < 1 month launch
                        model = await cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_week), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-6))
                    {
                        // < 6 month launch
                        model = await cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_month), cancellationToken);
                    }
                    else
                    {
                        // > 6 month launch
                        model = await cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_year), cancellationToken);
                    }
                }

                return model?.Data;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
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
                var tmdb_rating = req.GetQueryParameters()["tmdb_rating"];
                var title = req.GetQueryParameters()["title"];
                DateTime.TryParseExact(req.GetQueryParameters()["release_date"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime release_date);
                model = await cacheRepo.Get<Ratings>($"rating_new_{id}", cancellationToken);

                if (model == null)
                {
                    var scraping = new ScrapingRatings();
                    var obj = scraping.GetShowData(id, tmdb_rating, title, release_date.Year.ToString());
                    if (obj == null) return null;

                    if (release_date.Date == DateTime.MinValue.Date || release_date.Date == DateTime.MaxValue.Date)
                    {
                        //invalid date
                        model = await cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1))
                    {
                        // < 1 month launch
                        model = await cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_week), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-6))
                    {
                        // < 6 month launch
                        model = await cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_month), cancellationToken);
                    }
                    else
                    {
                        // > 6 month launch
                        model = await cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_year), cancellationToken);
                    }
                }

                return model?.Data;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
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
                    using var http = new HttpClient();
                    var obj = await http.GetReviewsByImdb8<RootMetacritic>(id, cancellationToken);
                    if (obj == null) return null;

                    var newModel = new ReviewModel();

                    foreach (var item in obj?.data?.title?.metacritic?.reviews?.edges ?? [])
                    {
                        if (item == null) continue;
                        newModel.Items.Add(new Shared.Models.Reviews.Item(item.node?.site, item.node?.url, item.node?.reviewer, item.node?.score, item.node?.quote?.value));
                    }

                    if (release_date.Date == DateTime.MinValue.Date || release_date.Date == DateTime.MaxValue.Date)
                    {
                        //invalid date
                        model = await cacheRepo.Add(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1))
                    {
                        // < 1 month launch
                        model = await cacheRepo.Add(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_week), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-6))
                    {
                        // < 6 month launch
                        model = await cacheRepo.Add(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_month), cancellationToken);
                    }
                    else
                    {
                        // > 6 month launch
                        model = await cacheRepo.Add(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_year), cancellationToken);
                    }
                }

                return model?.Data;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
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

                if (model == null)
                {
                    using var http = new HttpClient();
                    var url = $"https://internal-prod.apigee.fandom.net/v1/xapi/composer/metacritic/pages/shows-critic-reviews/{title}/web?filter=all&sort=score&apiKey=1MOZgmNFxvmljaQR1X9KAij9Mo4xAY3u";
                    var obj = await http.Get<MetaCriticScraping>(url, cancellationToken);
                    if (obj == null) return null;
                    if (obj.meta?.title == "undefined critic reviews") return null;

                    var newModel = new ReviewModel();

                    foreach (var item in obj.components.Find(w => w.meta?.componentName == "critic-reviews")?.data?.items ?? [])
                    {
                        if (item == null) continue;
                        newModel.Items.Add(new Shared.Models.Reviews.Item(item.publicationName, item.url, item.author, item.score, item.quote));
                    }

                    if (release_date.Date == DateTime.MinValue.Date || release_date.Date == DateTime.MaxValue.Date)
                    {
                        //invalid date
                        model = await cacheRepo.Add(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_day), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-1))
                    {
                        // < 1 month launch
                        model = await cacheRepo.Add(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_week), cancellationToken);
                    }
                    else if (release_date > DateTime.Now.AddMonths(-6))
                    {
                        // < 6 month launch
                        model = await cacheRepo.Add(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_month), cancellationToken);
                    }
                    else
                    {
                        // > 6 month launch
                        model = await cacheRepo.Add(new MetaCriticCache(newModel, $"review_{id}", ttlCache.one_year), cancellationToken);
                    }
                }

                return model?.Data;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}