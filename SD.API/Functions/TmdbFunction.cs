using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SD.API.Core;
using SD.Shared.Helper;
using SD.Shared.Modal.Tmdb;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Functions
{
    public class TmdbFunction
    {
        private readonly IConfiguration configuration;
        private readonly IRepository _repo;

        public TmdbFunction(IConfiguration Configuration, IRepository repo)
        {
            configuration = Configuration;
            _repo = repo;
        }

        [FunctionName("GetTmdbId")]
        [OpenApiOperation("GetTmdbId", new[] { "TMDB" }, Summary = "Get TMDB.ID from IMDB.ID (exclusive for tv shows)")]
        [OpenApiParameter("external_id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "IMDB.ID")]
        //[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string))]
        public async Task<IActionResult> GetTmdbId(
            [HttpTrigger(AuthorizationLevel.Anonymous, FunctionMethod.GET, Route = "TMDB/GetTmdbId/{external_id}")] HttpRequest req,
            string external_id, ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            var options = configuration.GetSection(TmdbOptions.Section).Get<TmdbOptions>();

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", options.ApiKey },
                { "external_source", "imdb_id" }
            };

            using var http = new HttpClient();

            var result = await http.Get<FindByImdb>(options.BaseUri + "find/" + external_id.ConfigureParameters(parameter), source.Token);

            return new OkObjectResult(result.tv_results.First().id.ToString());
        }
    }
}