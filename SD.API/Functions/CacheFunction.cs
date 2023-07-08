using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Scraping;
using SD.Shared.Core.Models;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.News;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Trailers;

namespace SD.API.Functions
{
    public class CacheFunction
    {
        private readonly CosmosCacheRepository _cacheRepo;

        public CacheFunction(CosmosCacheRepository cacheRepo)
        {
            _cacheRepo = cacheRepo;
        }

        [Function("CacheNew")]
        public async Task<CacheDocument<NewsModel>?> CacheNew(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/News")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var mode = req.GetQueryParameters()["mode"];
                var model = await _cacheRepo.Get<NewsModel>($"lastnews_{mode}", cancellationToken);

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

                    var compactResult = await _cacheRepo.Add(new FlixsterCache(compactModels, "lastnews_compact"), cancellationToken);

                    //full

                    var fullModels = new NewsModel();

                    foreach (var item in obj.data?.newsStories ?? Enumerable.Empty<NewsStory>())
                    {
                        if (item == null) continue;
                        fullModels.Items.Add(new Shared.Models.News.Item(item.id, item.title, item.mainImage?.url, item.link));
                    }

                    var fullResult = await _cacheRepo.Add(new FlixsterCache(fullModels, "lastnews_full"), cancellationToken);

                    if (mode == "compact")
                        return compactResult;
                    else
                        return fullResult;
                }
                else
                {
                    return model;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("CacheRatings")]
        public async Task<CacheDocument<Ratings>?> CacheRatings(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Ratings")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                CacheDocument<Ratings>? model;

                var id = req.GetQueryParameters()["id"];
                _ = DateTime.TryParse(req.GetQueryParameters()["release_date"], out DateTime release_date);
                model = await _cacheRepo.Get<Ratings>($"rating_{id}", cancellationToken);

                if (model == null)
                {
                    using var http = new HttpClient();
                    var obj = await http.Get<Ratings>(ImdbOptions.BaseUri + $"Ratings/{ImdbOptions.ApiKey}/{id}", cancellationToken);
                    if (obj == null) return null;

                    if (string.IsNullOrEmpty(obj.errorMessage))
                    {
                        if (release_date.Date == DateTime.MinValue.Date || release_date.Date == DateTime.MaxValue.Date)
                        {
                            model = await _cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_day), cancellationToken);
                        }
                        else if (release_date > DateTime.Now.AddMonths(-1))
                        {
                            model = await _cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_week), cancellationToken);
                        }
                        else if (release_date > DateTime.Now.AddMonths(-6))
                        {
                            model = await _cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_month), cancellationToken);
                        }
                        else
                        {
                            model = await _cacheRepo.Add(new RatingsCache(id, obj, ttlCache.one_year), cancellationToken);
                        }
                    }
                    else
                    {
                        model = new RatingsCache(id, obj, ttlCache.one_day);
                    }
                }

                return model;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("CacheTrailers")]
        public async Task<CacheDocument<TrailerModel>?> CacheTrailers(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Trailers")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var mode = req.GetQueryParameters()["mode"];
                var model = await _cacheRepo.Get<TrailerModel>($"lasttrailers_{mode}", cancellationToken);

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
                        compactModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title, item.thumbnails.First().url));
                    }

                    var compactResult = await _cacheRepo.Add(new YoutubeCache(compactModels, "lasttrailers_compact"), cancellationToken);

                    //full

                    var fullModels = new TrailerModel();

                    foreach (var item in obj.contents.Select(s => s.video))
                    {
                        if (item == null) continue;
                        fullModels.Items.Add(new Shared.Models.Trailers.Item(item.videoId, item.title, item.thumbnails[2].url));
                    }

                    var fullResult = await _cacheRepo.Add(new YoutubeCache(fullModels, "lasttrailers_full"), cancellationToken);

                    if (mode == "compact")
                        return compactResult;
                    else
                        return fullResult;
                }
                else
                {
                    return model;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("ImdbPopularMovies")]
        public async Task<CacheDocument<MostPopularData>?> ImdbPopularMovies(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/ImdbPopularMovies")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var mode = req.GetQueryParameters()["mode"];
                var model = await _cacheRepo.Get<MostPopularData>($"popularmovies_{mode}", cancellationToken);

                if (model == null)
                {
                    //var parameter = new Dictionary<string, string>() { { "apiKey", ImdbOptions.ApiKey } };

                    using var http = new HttpClient();
                    var scraping = new MostPopularMovies();
                    //var obj = await http.Get<MostPopularData>(ImdbOptions.BaseUri + "MostPopularMovies".ConfigureParameters(parameter), cancellationToken);
                    var obj = await scraping.GetMovieData();
                    if (obj == null) return null;

                    //compact

                    var compactModels = new MostPopularData() { ErrorMessage = obj.ErrorMessage };

                    foreach (var item in obj.Items.Take(10))
                    {
                        if (item == null) continue;
                        compactModels.Items.Add(item);
                    }

                    var compactResult = await _cacheRepo.Add(new MostPopularDataCache(compactModels, "popularmovies_compact"), cancellationToken);

                    //full

                    var fullModels = new MostPopularData() { ErrorMessage = obj.ErrorMessage };

                    foreach (var item in obj.Items)
                    {
                        if (item == null) continue;
                        fullModels.Items.Add(item);
                    }

                    var fullResult = await _cacheRepo.Add(new MostPopularDataCache(fullModels, "popularmovies_full"), cancellationToken);

                    if (mode == "compact")
                        return compactResult;
                    else
                        return fullResult;
                }
                else
                {
                    return model;
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("ImdbPopularTVs")]
        public async Task<CacheDocument<MostPopularData>?> ImdbPopularTVs(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/ImdbPopularTVs")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var model = await _cacheRepo.Get<MostPopularData>("populartvs", cancellationToken);

                if (model == null)
                {
                    //var parameter = new Dictionary<string, string>() { { "apiKey", ImdbOptions.ApiKey } };

                    using var http = new HttpClient();
                    var scraping = new MostPopularMovies();
                    //var obj = await http.Get<MostPopularData>(ImdbOptions.BaseUri + "MostPopularTVs".ConfigureParameters(parameter), cancellationToken);
                    var obj = await scraping.GetTvData();
                    if (obj == null) return null;

                    model = await _cacheRepo.Add(new MostPopularDataCache(obj, "populartvs"), cancellationToken);
                }

                return model;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("ImdbComingMovies")]
        public async Task<CacheDocument<NewMovieData>?> ImdbComingMovies(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/ImdbComingMovies")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var model = await _cacheRepo.Get<NewMovieData>("comingmovies", cancellationToken);

                if (model == null)
                {
                    var parameter = new Dictionary<string, string>() { { "apiKey", ImdbOptions.ApiKey } };

                    using var http = new HttpClient();
                    var obj = await http.Get<NewMovieData>(ImdbOptions.BaseUri + "ComingSoon".ConfigureParameters(parameter), cancellationToken);
                    if (obj == null) return null;

                    model = await _cacheRepo.Add(new NewMovieDataCache(obj, "comingmovies"), cancellationToken);
                }

                return model;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("CacheReviews")]
        public async Task<CacheDocument<ReviewModel>?> CacheReviews(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Reviews")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                CacheDocument<ReviewModel>? model;

                var id = req.GetQueryParameters()["id"];
                //_ = DateTime.TryParse(req.GetQueryParameters()["release_date"], out DateTime release_date);
                model = await _cacheRepo.Get<ReviewModel>($"review_{id}", cancellationToken);

                if (model == null)
                {
                    using var http = new HttpClient();
                    var obj = await http.GetReviewsByImdb8<MetaCritic>(id, cancellationToken);
                    if (obj == null) return null;

                    var newModel = new ReviewModel();

                    foreach (var item in obj.reviews)
                    {
                        if (item == null) continue;
                        newModel.Items.Add(new Shared.Models.Reviews.Item(item.reviewSite, item.reviewUrl, item.reviewer, item.score, item.quote));
                    }

                    model = await _cacheRepo.Add(new MetaCriticCache(newModel, $"review_{id}"), cancellationToken);
                }

                return model;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}