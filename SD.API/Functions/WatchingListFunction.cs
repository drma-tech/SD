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
    public class WatchingListFunction
    {
        private readonly IRepository _repo;

        public WatchingListFunction(IRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("WatchingListGet")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.GET, Route = "WatchingList/Get")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var result = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), req.GetUserId(), source.Token);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("WatchingListAdd")]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "WatchingList/Add/{MediaType}")] HttpRequest req,
            string MediaType, ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var obj = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), req.GetUserId(), source.Token);
                var newItem = await req.GetParameterObjectPublic<WatchingListItem>(source.Token);

                if (obj == null)
                {
                    obj = new WatchingList
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

        [FunctionName("WatchingListRemove")]
        public async Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "WatchingList/Remove/{MediaType}/{CollectionId}/{TmdbId}")] HttpRequest req,
            string MediaType, string CollectionId, string TmdbId, ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var obj = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), req.GetUserId(), source.Token);

                if (obj == null)
                {
                    obj = new WatchingList
                    {
                        DtInsert = DateTimeOffset.UtcNow
                    };

                    obj.SetIds(req.GetUserId());
                }
                else
                {
                    obj.DtUpdate = DateTimeOffset.UtcNow;
                }

                obj.RemoveItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), CollectionId, TmdbId == "null" ? null : TmdbId);

                obj = await _repo.Upsert(obj, source.Token);

                return new OkObjectResult(obj);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("WatchingListSync")]
        public async Task<IActionResult> Sync(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "WatchingList/Sync/{MediaType}")] HttpRequest req,
            string MediaType, ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var obj = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), req.GetUserId(), source.Token);

                if (obj == null)
                {
                    obj = new WatchingList
                    {
                        DtInsert = DateTimeOffset.UtcNow
                    };

                    obj.SetIds(req.GetUserId());
                }
                else
                {
                    obj.DtUpdate = DateTimeOffset.UtcNow;
                }

                var type = (MediaType)Enum.Parse(typeof(MediaType), MediaType);

                if (type == Shared.Enums.MediaType.movie)
                {
                    obj.MovieSyncDate = DateTime.Now;
                }
                else
                {
                    obj.ShowSyncDate = DateTime.Now;
                }

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