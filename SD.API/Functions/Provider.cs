using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SD.API.Core;
using SD.Shared.Modal;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Functions
{
    public class Provider
    {
        private readonly IRepository _repo;

        public Provider(IRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("GetAll")]
        [OpenApiOperation("GetAll", new[] { "Provider" }, Summary = "Recovery all the streamings providers")]
        //[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(AllProviders), Description = "The OK response")]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, FunctionMethod.GET, Route = "Provider/GetAll")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            var result = await _repo.Get<AllProviders>("providers", "providers", source.Token);

            return new OkObjectResult(result);
        }

        [FunctionName("Post")]
        [OpenApiOperation(operationId: "Post", new[] { "Provider" }, Summary = "Update the streamings providers")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Description = "The OK response")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Admin, FunctionMethod.POST, Route = "Provider/Post")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            var obj = await req.GetParameterObject<AllProviders>(source.Token);

            var result = await _repo.Update(obj, source.Token);

            return new OkObjectResult(result);
        }
    }
}