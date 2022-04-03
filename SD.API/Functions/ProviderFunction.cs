using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SD.API.Core;
using SD.Shared.Core;
using SD.Shared.Helper;
using SD.Shared.Modal;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Tmdb;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Functions
{
    public class ProviderFunction
    {
        private readonly IConfiguration configuration;
        private readonly IRepository _repo;

        public ProviderFunction(IConfiguration Configuration, IRepository repo)
        {
            configuration = Configuration;
            _repo = repo;
        }

        [FunctionName("GetAll")]
        [OpenApiOperation("GetAll", new[] { "Provider" }, Summary = "Recovery all the streamings providers")]
        //[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Provider>), Description = "The OK response")]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, FunctionMethod.GET, Route = "Provider/GetAll")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            var result = await _repo.Get<AllProviders>("providers", "providers", source.Token);

            return new OkObjectResult(result.Items);
        }

        [FunctionName("Post")]
        [OpenApiOperation("Post", new[] { "Provider" }, Summary = "Update the streamings providers")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Provider>), Description = "The OK response")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Admin, FunctionMethod.POST, Route = "Provider/Post")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            var AllProviders = await _repo.Get<AllProviders>("providers", "providers", source.Token);
            var providers = await req.GetParameterGenericObject<List<Provider>>(source.Token);

            AllProviders.DtUpdate = System.DateTimeOffset.UtcNow;
            AllProviders.Items = providers.OrderBy(o => int.Parse(o.id)).ToList();
            await _repo.Update(AllProviders, source.Token, ru_limit: 300);

            return new OkObjectResult(AllProviders.Items);
        }

        [FunctionName("UpdateAllProvider")]
        [OpenApiOperation("UpdateAllProvider", new[] { "Provider" }, Summary = "Syncronize all providers")]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Provider>), Description = "The OK response")]
        public async Task<IActionResult> UpdateAllProvider(
           [HttpTrigger(AuthorizationLevel.Anonymous, FunctionMethod.PUT, Route = "Provider/UpdateAllProvider")] HttpRequest req,
           ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            var options = configuration.GetSection(TmdbOptions.Section).Get<TmdbOptions>();
            var result = new List<Provider>();

            var AllProviders = await _repo.Get<AllProviders>("providers", "providers", source.Token);
            var details = AllProviders.Items;

            foreach (var region in EnumHelper.GetArray<Region>())
            {
                var parameter = new Dictionary<string, string>()
                {
                    { "api_key", options.ApiKey },
                    { "language", Language.enUS.GetName(false) },
                    { "watch_region", region.ToString() }
                };

                using (var http = new HttpClient())
                {
                    var movies = await http.Get<TMDB_AllProviders>(options.BaseUri + "watch/providers/movie".ConfigureParameters(parameter), source.Token);
                    AddProvider(result, movies.results, details, region, MediaType.movie);

                    var tvs = await http.Get<TMDB_AllProviders>(options.BaseUri + "watch/providers/tv".ConfigureParameters(parameter), source.Token);
                    AddProvider(result, tvs.results, details, region, MediaType.tv);
                }
            }

            var _new = AllProviders.DtUpdate == null && AllProviders.DtInsert.AddDays(-7) > System.DateTimeOffset.UtcNow;
            var _old = AllProviders.DtUpdate != null && AllProviders.DtUpdate.Value.AddDays(-7) > System.DateTimeOffset.UtcNow;

            if (_new || _old)
            {
                AllProviders.DtUpdate = System.DateTimeOffset.UtcNow;
                AllProviders.Items = result.OrderBy(o => int.Parse(o.id)).ToList();
                await _repo.Update(AllProviders, source.Token, ru_limit: 300);
            }

            return new OkObjectResult(AllProviders.Items);
        }

        private static void AddProvider(List<Provider> final_list, List<ProviderBase> new_providers, List<Provider> current_providers, Region region, MediaType type)
        {
            foreach (var item in new_providers)
            {
                var detail = current_providers.FirstOrDefault(f => f.id == item.provider_id.ToString());
                var new_item = final_list.FirstOrDefault(f => f.id == item.provider_id.ToString());

                if (new_item == null)
                {
                    final_list.Add(new Provider
                    {
                        //api
                        id = item.provider_id.ToString(),
                        name = item.provider_name,
                        priority = item.display_priority,
                        logo_path = item.logo_path,
                        //own data
                        description = detail?.description,
                        link = detail?.link,
                        head_language = detail?.head_language,
                        plans = detail?.plans ?? new(),
                        models = detail?.models ?? new(),
                        empty_catalog = detail?.empty_catalog,
                        enabled = detail?.enabled ?? true,
                        //api by regions
                        regions = new List<Region> { region },
                        types = new List<MediaType> { type }
                    });
                }
                else
                {
                    if (!new_item.regions.Any(a => a == region))
                    {
                        new_item.regions.Add(region);
                    }
                    new_item.regions = new_item.regions.OrderBy(o => o).ToList();

                    if (!new_item.types.Any(a => a == type))
                    {
                        new_item.types.Add(type);
                    }
                    new_item.types = new_item.types.OrderBy(o => o).ToList();
                }
            }
        }
    }
}