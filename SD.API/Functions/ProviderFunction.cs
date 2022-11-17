using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SD.Shared.Model.List.Tmdb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Functions
{
    public class ProviderFunction
    {
        private readonly IRepository _repo;

        public ProviderFunction(IRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("PublicProviderGetAll")]
        public async Task<IActionResult> GetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, FunctionMethod.GET, Route = "Public/Provider/GetAll")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var result = await _repo.Get<AllProviders>("providers", "providers", source.Token);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("ProviderPost")]
        public async Task<IActionResult> Post(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "Provider/Post")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var AllProviders = await _repo.Get<AllProviders>("providers", "providers", source.Token);
                var providers = await req.GetParameterObjectPublic<AllProviders>(source.Token);

                if (AllProviders != null)
                {
                    AllProviders.DtUpdate = DateTimeOffset.UtcNow;
                    AllProviders.Items = providers.Items.OrderBy(o => int.Parse(o.id ?? "0")).ToList();
                    await _repo.Upsert(AllProviders, source.Token);
                }

                return new OkObjectResult(AllProviders);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("ProviderSyncProviders")]
        public async Task<IActionResult> SyncProviders(
           [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.PUT, Route = "Provider/SyncProviders")] HttpRequest req,
           ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

                var result = new List<ProviderModel>();

                var AllProviders = await _repo.Get<AllProviders>("providers", "providers", source.Token);

                if (AllProviders != null)
                {
                    var details = AllProviders.Items;

                    foreach (var region in EnumHelper.GetArray<Region>())
                    {
                        var parameter = new Dictionary<string, string>()
                        {
                            { "api_key", TmdbOptions.ApiKey },
                            { "language", Language.enUS.GetName(false) ?? "en-US" },
                            { "watch_region", region.ToString() }
                        };

                        using (var http = new HttpClient())
                        {
                            var movies = await http.Get<TMDB_AllProviders>(TmdbOptions.BaseUri + "watch/providers/movie".ConfigureParameters(parameter), source.Token);
                            if (movies != null) AddProvider(result, movies.results, details, region, MediaType.movie);

                            var tvs = await http.Get<TMDB_AllProviders>(TmdbOptions.BaseUri + "watch/providers/tv".ConfigureParameters(parameter), source.Token);
                            if (tvs != null) AddProvider(result, tvs.results, details, region, MediaType.tv);
                        }
                    }

                    var _new = AllProviders.DtUpdate == null && AllProviders.DtInsert < DateTimeOffset.UtcNow.AddDays(-7);
                    var _old = AllProviders.DtUpdate != null && AllProviders.DtUpdate.Value < DateTimeOffset.UtcNow.AddDays(-7);

                    if (_new || _old)
                    {
                        AllProviders.DtUpdate = DateTimeOffset.UtcNow;
                        AllProviders.Items = result.OrderBy(o => int.Parse(o.id ?? "0")).ToList();
                        await _repo.Upsert(AllProviders, source.Token);
                    }
                }

                return new OkObjectResult(AllProviders);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        private static void AddProvider(List<ProviderModel> final_list, List<ProviderBase> new_providers, List<ProviderModel> current_providers, Region region, MediaType type)
        {
            foreach (var item in new_providers)
            {
                var detail = current_providers.FirstOrDefault(f => f.id == item.provider_id.ToString());
                var new_item = final_list.FirstOrDefault(f => f.id == item.provider_id.ToString());

                if (new_item == null)
                {
                    final_list.Add(new ProviderModel
                    {
                        //api
                        id = item.provider_id.ToString(),
                        name = item.provider_name,
                        priority = item.display_priority,
                        logo_path = item.logo_path,
                        //own data (manual update)
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