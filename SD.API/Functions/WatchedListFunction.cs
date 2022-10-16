using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SD.API.Core;
using SD.Shared.Core;
using SD.Shared.Model;
using System;
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

                var result = await _repo.Get<WatchedList>(CosmosType.WatchedList + ":" + req.GetUserId(), req.GetUserId(), source.Token);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("WatchedListPost")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "WatchedList/Post")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var myProviders = await _repo.Get<WatchedList>(CosmosType.WatchedList + ":" + req.GetUserId(), req.GetUserId(), source.Token);
                var newItem = await req.GetParameterGenericObject<WatchedList>(source.Token);

                if (myProviders == null)
                {
                    myProviders = new WatchedList
                    {
                        DtInsert = DateTimeOffset.UtcNow
                    };
                }
                else
                {
                    myProviders.DtUpdate = DateTimeOffset.UtcNow;
                }

                myProviders.SetIds(req.GetUserId());

                myProviders.Items = newItem.Items;
                myProviders = await _repo.Upsert(myProviders, source.Token);

                return new OkObjectResult(myProviders);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }
    }
}