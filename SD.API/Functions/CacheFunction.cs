using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.News;
using SD.Shared.Models.Trailers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Functions
{
    public class CacheFunction
    {
        private readonly CosmosCacheRepository _repo;

        public CacheFunction(CosmosCacheRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("CacheNewsGet")]
        public async Task<IActionResult> GetNews(
           [HttpTrigger(AuthorizationLevel.Anonymous, FunctionMethod.GET, Route = "Public/Cache/News/Get")] HttpRequest req,
           ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);
            try
            {
                var result = await _repo.Get<Flixster>("lastnews", cancellationToken);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("CacheNewsAdd")]
        public async Task<IActionResult> AddNews(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "Public/Cache/News/Add")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            try
            {
                var item = await req.GetParameterObjectPublic<FlixsterCache>(source.Token);

                var model = await _repo.Add(item, cancellationToken);

                return new OkObjectResult(model);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("CacheRatingsGet")]
        public async Task<IActionResult> GetRatings(
           [HttpTrigger(AuthorizationLevel.Anonymous, FunctionMethod.GET, Route = "Public/Cache/Ratings/Get")] HttpRequest req,
           ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);
            try
            {
                var id = req.GetQueryParameterDictionary()["id"];
                var result = await _repo.Get<Ratings>($"rating_{id}", cancellationToken);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("CacheRatingsAdd")]
        public async Task<IActionResult> AddRatings(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "Public/Cache/Ratings/Add")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            try
            {
                var item = await req.GetParameterObjectPublic<RatingsCache>(source.Token);

                var model = await _repo.Add(item, cancellationToken);

                return new OkObjectResult(model);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("CacheTrailersGet")]
        public async Task<IActionResult> GetTrailers(
           [HttpTrigger(AuthorizationLevel.Anonymous, FunctionMethod.GET, Route = "Public/Cache/Trailers/Get")] HttpRequest req,
           ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);
            try
            {
                var result = await _repo.Get<Youtube>("lasttrailers", cancellationToken);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("CacheTrailersAdd")]
        public async Task<IActionResult> AddTrailers(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "Public/Cache/Trailers/Add")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            try
            {
                var item = await req.GetParameterObjectPublic<YoutubeCache>(source.Token);

                var model = await _repo.Add(item, cancellationToken);

                return new OkObjectResult(model);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }
    }
}