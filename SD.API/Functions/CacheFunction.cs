using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.Shared.Core.Models;
using SD.Shared.Model.List.Imdb;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.News;
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
        public async Task<HttpResponseData> CacheNew(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/News")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var model = await _cacheRepo.Get<Flixster>("lastnews", cancellationToken);

                if (model == null)
                {
                    using var http = new HttpClient();
                    var obj = await http.GetNewsByFlixter<Flixster>(cancellationToken);
                    if (obj == null) return await req.ProcessObject(obj, cancellationToken);

                    model = await _cacheRepo.Add(new FlixsterCache(obj), cancellationToken);
                }

                return await req.ProcessObject(model, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("CacheRatings")]
        public async Task<HttpResponseData> CacheRatings(
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
                    if (obj == null) return await req.ProcessObject(obj, cancellationToken);

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

                return await req.ProcessObject(model, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("CacheTrailers")]
        public async Task<HttpResponseData> CacheTrailers(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/Trailers")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var model = await _cacheRepo.Get<Youtube>("lasttrailers", cancellationToken);

                if (model == null)
                {
                    using var http = new HttpClient();
                    var obj = await http.GetTrailersByYoutubeSearch<Youtube>(cancellationToken);
                    if (obj == null) return await req.ProcessObject(obj, cancellationToken);

                    model = await _cacheRepo.Add(new YoutubeCache(obj), cancellationToken);
                }

                return await req.ProcessObject(model, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("ImdbPopularMovies")]
        public async Task<HttpResponseData> ImdbPopularMovies(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/ImdbPopularMovies")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var model = await _cacheRepo.Get<MostPopularData>("popularmovies", cancellationToken);

                if (model == null)
                {
                    var parameter = new Dictionary<string, string>() { { "apiKey", ImdbOptions.ApiKey } };

                    using var http = new HttpClient();
                    var obj = await http.Get<MostPopularData>(ImdbOptions.BaseUri + "MostPopularMovies".ConfigureParameters(parameter), cancellationToken);

                    //processar as imagens

                    if (obj == null) return await req.ProcessObject(obj, cancellationToken);

                    model = await _cacheRepo.Add(new MostPopularDataCache(obj, "popularmovies"), cancellationToken);
                }

                return await req.ProcessObject(model, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("ImdbPopularTVs")]
        public async Task<HttpResponseData> ImdbPopularTVs(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Cache/ImdbPopularTVs")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var model = await _cacheRepo.Get<MostPopularData>("populartvs", cancellationToken);

                if (model == null)
                {
                    var parameter = new Dictionary<string, string>() { { "apiKey", ImdbOptions.ApiKey } };

                    using var http = new HttpClient();
                    var obj = await http.Get<MostPopularData>(ImdbOptions.BaseUri + "MostPopularTVs".ConfigureParameters(parameter), cancellationToken);

                    //processar as imagens

                    if (obj == null) return await req.ProcessObject(obj, cancellationToken);

                    model = await _cacheRepo.Add(new MostPopularDataCache(obj, "populartvs"), cancellationToken);
                }

                return await req.ProcessObject(model, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }
    }
}