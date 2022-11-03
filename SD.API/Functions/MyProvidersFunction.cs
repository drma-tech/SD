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
    public class MyProvidersFunction
    {
        private readonly IRepository _repo;

        public MyProvidersFunction(IRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("MyProvidersGet")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.GET, Route = "MyProviders/Get")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var result = await _repo.Get<MyProviders>(DocumentType.MyProvider + ":" + req.GetUserId(), req.GetUserId(), source.Token);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("MyProvidersPost")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "MyProviders/Post")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var myProviders = await _repo.Get<MyProviders>(DocumentType.MyProvider + ":" + req.GetUserId(), req.GetUserId(), source.Token);
                var newItem = await req.GetParameterObject<MyProviders>(source.Token);

                if (myProviders == null)
                {
                    myProviders = new MyProviders
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