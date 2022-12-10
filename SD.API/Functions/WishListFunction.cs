using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Functions
{
    public class WishListFunction
    {
        private readonly IRepository _repo;

        public WishListFunction(IRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("WishListGet")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.GET, Route = "WishList/Get")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var result = await _repo.Get<WishList>(DocumentType.WishList + ":" + req.GetUserId(), req.GetUserId(), source.Token);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("WishListAdd")]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "WishList/Add/{MediaType}")] HttpRequest req,
            string MediaType, ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var obj = await _repo.Get<WishList>(DocumentType.WishList + ":" + req.GetUserId(), req.GetUserId(), source.Token);
                var newItem = await req.GetParameterObjectPublic<WishListItem>(source.Token);

                if (obj == null)
                {
                    obj = new WishList
                    {
                        DtInsert = DateTimeOffset.UtcNow
                    };

                    obj.SetIds(req.GetUserId());
                }
                else
                {
                    obj.DtUpdate = DateTimeOffset.UtcNow;
                }

                obj.AddItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), newItem);

                obj = await _repo.Upsert(obj, source.Token);

                return new OkObjectResult(obj);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("WishListRemove")]
        public async Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "WishList/Remove/{MediaType}/{TmdbId}")] HttpRequest req,
            string MediaType, string TmdbId, ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var obj = await _repo.Get<WishList>(DocumentType.WishList + ":" + req.GetUserId(), req.GetUserId(), source.Token);

                if (obj == null)
                {
                    obj = new WishList
                    {
                        DtInsert = DateTimeOffset.UtcNow
                    };

                    obj.SetIds(req.GetUserId());
                }
                else
                {
                    obj.DtUpdate = DateTimeOffset.UtcNow;
                }

                obj.RemoveItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), TmdbId);

                obj = await _repo.Upsert(obj, source.Token);

                return new OkObjectResult(obj);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }
    }
}