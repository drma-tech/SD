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
                    var scraping = new MostPopularMovies();
                    var obj = scraping.GetMovieData();
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
                    using var http = new HttpClient();
                    var scraping = new MostPopularMovies();
                    var obj = scraping.GetTvData();
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

        [Function("CacheMovieReviews")]
        public async Task<CacheDocument<ReviewModel>?> CacheMovieReviews(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Reviews/Movies")] HttpRequestData req, CancellationToken cancellationToken)
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

        [Function("CacheShowReviews")]
        public async Task<CacheDocument<ReviewModel>?> CacheShowReviews(
          [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Reviews/Shows")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                //https://fandom-prod.apigee.net/v1/xapi/shows/metacritic/game-of-thrones/web?apiKey=1MOZgmNFxvmljaQR1X9KAij9Mo4xAY3u
                //https://fandom-prod.apigee.net/v1/xapi/reviews/metacritic/critic/shows/game-of-thrones/web?apiKey=1MOZgmNFxvmljaQR1X9KAij9Mo4xAY3u

                CacheDocument<ReviewModel>? model;

                var id = req.GetQueryParameters()["id"];
                var title = req.GetQueryParameters()["title"];
                //_ = DateTime.TryParse(req.GetQueryParameters()["release_date"], out DateTime release_date);
                model = await _cacheRepo.Get<ReviewModel>($"review_{id}", cancellationToken);

                if (model == null)
                {
                    //var scraping = new ShowsReview();
                    //var obj = await scraping.GetTvReviews(title);
                    //if (obj == null) return null;

                    using var http = new HttpClient();
                    var obj = await http.Get<MetaCriticScraping>($"https://fandom-prod.apigee.net/v1/xapi/reviews/metacritic/critic/shows/{title}/web?apiKey=1MOZgmNFxvmljaQR1X9KAij9Mo4xAY3u", cancellationToken);
                    if (obj == null) return null;

                    var newModel = new ReviewModel();

                    foreach (var item in obj.data?.items ?? new())
                    {
                        if (item == null) continue;
                        newModel.Items.Add(new Shared.Models.Reviews.Item(item.publicationName, item.url, item.author, item.score, item.quote));
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