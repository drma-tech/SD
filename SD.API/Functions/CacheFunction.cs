using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.Shared.Core.Models;
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
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Method.POST, Route = "Public/Cache/News")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                CacheDocument<Flixster>? model;

                if (req.Method == Method.GET)
                {
                    model = await _cacheRepo.Get<Flixster>("lastnews", cancellationToken);
                }
                else
                {
                    var body = await req.GetPublicBody<FlixsterCache>(cancellationToken);
                    model = await _cacheRepo.Add(body, cancellationToken);
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
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Method.POST, Route = "Public/Cache/Ratings")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                CacheDocument<Ratings>? model;

                if (req.Method == Method.GET)
                {
                    var id = req.GetQueryParameters()["id"];
                    model = await _cacheRepo.Get<Ratings>($"rating_{id}", cancellationToken);
                }
                else
                {
                    var body = await req.GetPublicBody<RatingsCache>(cancellationToken);
                    model = await _cacheRepo.Add(body, cancellationToken);
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
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Method.POST, Route = "Public/Cache/Trailers")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                CacheDocument<Youtube>? model = null;

                if (req.Method == Method.GET)
                {
                    model = await _cacheRepo.Get<Youtube>("lasttrailers", cancellationToken);
                }
                else
                {
                    var body = await req.GetPublicBody<YoutubeCache>(cancellationToken);
                    model = await _cacheRepo.Add(body, cancellationToken);
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