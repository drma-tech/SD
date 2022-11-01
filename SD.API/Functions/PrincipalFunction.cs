using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SD.Shared.Model.Auth;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Functions
{
    public class PrincipalFunction
    {
        private readonly IRepository _repo;

        public PrincipalFunction(IRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("PrincipalGet")]
        public async Task<IActionResult> Get(
           [HttpTrigger(AuthorizationLevel.Anonymous, FunctionMethod.GET, Route = "Principal/Get")] HttpRequest req,
           ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);
            try
            {
                var result = await _repo.Get<ClientePrincipal>(DocumentType.Principal + ":" + req.GetUserId(), req.GetUserId(), cancellationToken);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("PrincipalAdd")]
        public async Task<IActionResult> Add(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "Principal/Add")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            try
            {
                var item = await req.GetParameterGenericObject<ClientePrincipal>(source.Token);

                var result = await _repo.Upsert(item, cancellationToken);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }
    }
}