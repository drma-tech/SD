using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Functions
{
    public class WatchedListFunction
    {
        private readonly IRepository _repo;

        public WatchedListFunction(IRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("WatchedListGet")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.GET, Route = "WatchedList/Get")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var result = await _repo.Get<WatchedList>(DocumentType.WatchedList + ":" + req.GetUserId(), req.GetUserId(), source.Token);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("WatchedListAdd")]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "WatchedList/Add/{MediaType}/{TmdbId}")] HttpRequest req,
            string MediaType, string TmdbId, ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var obj = await _repo.Get<WatchedList>(DocumentType.WatchedList + ":" + req.GetUserId(), req.GetUserId(), source.Token);

                if (obj == null)
                {
                    obj = new WatchedList
                    {
                        DtInsert = DateTimeOffset.UtcNow
                    };

                    obj.SetIds(req.GetUserId());
                }
                else
                {
                    obj.DtUpdate = DateTimeOffset.UtcNow;
                }

                var ids = TmdbId.Split(',');
                obj.AddItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), new HashSet<string>(ids));

                obj = await _repo.Upsert(obj, source.Token);

                return new OkObjectResult(obj);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("WatchedListRemove")]
        public async Task<IActionResult> Remove(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "WatchedList/Remove/{MediaType}/{TmdbId}")] HttpRequest req,
            string MediaType, string TmdbId, ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var obj = await _repo.Get<WatchedList>(DocumentType.WatchedList + ":" + req.GetUserId(), req.GetUserId(), source.Token);

                if (obj == null)
                {
                    obj = new WatchedList
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