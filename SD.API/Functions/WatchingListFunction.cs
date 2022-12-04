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

        [FunctionName("WatchingListPost")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "WatchingList/Post")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var item = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), req.GetUserId(), source.Token);
                var newItem = await req.GetParameterObject<WatchingList>(source.Token);

                if (item == null)
                {
                    item = new WatchingList
                    {
                        DtInsert = DateTimeOffset.UtcNow
                    };
                }
                else
                {
                    item.DtUpdate = DateTimeOffset.UtcNow;
                }

                item.SetIds(req.GetUserId());

                item.SetCollection(MediaType.movie, newItem.Movies);
                item.SetCollection(MediaType.tv, newItem.Shows);

                item = await _repo.Upsert(item, source.Token);

                return new OkObjectResult(item);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }
    }
}