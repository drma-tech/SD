using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Configuration;
using SD.API.Repository.Core;
using SD.Shared.Models.List.Tmdb;
using System.Net;

namespace SD.API.Functions
{
    public class ProviderFunction
    {
        private readonly IRepository _repo;

        public ProviderFunction(IRepository repo)
        {
            _repo = repo;
        }

        [OpenApiOperation("ProviderPost", "Azure (Cosmos DB)")]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(AllProviders))]
        [Function("ProviderPost")]
        public async Task<AllProviders?> Post(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "Provider/Post")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var AllProviders = await _repo.Get<AllProviders>("providers", new PartitionKey("providers"), cancellationToken);
                var providers = await req.GetPublicBody<AllProviders>(cancellationToken);

                if (AllProviders != null)
                {
                    AllProviders.Update();
                    AllProviders.Items = providers.Items.OrderBy(o => int.Parse(o.id ?? "0")).ToList();
                    await _repo.Upsert(AllProviders, cancellationToken);
                }

                return AllProviders;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [OpenApiOperation("ProviderSyncProviders", "Azure (Cosmos DB)")]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(AllProviders))]
        [Function("ProviderSyncProviders")]
        public async Task<AllProviders?> SyncProviders(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.PUT, Route = "Provider/SyncProviders")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var result = new List<ProviderModel>();

                var AllProviders = await _repo.Get<AllProviders>("providers", new PartitionKey("providers"), cancellationToken);

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
                            var movies = await http.Get<TMDB_AllProviders>(TmdbOptions.BaseUri + "watch/providers/movie".ConfigureParameters(parameter), cancellationToken);
                            if (movies != null) AddProvider(result, movies.results, details, region, MediaType.movie);

                            var tvs = await http.Get<TMDB_AllProviders>(TmdbOptions.BaseUri + "watch/providers/tv".ConfigureParameters(parameter), cancellationToken);
                            if (tvs != null) AddProvider(result, tvs.results, details, region, MediaType.tv);
                        }
                    }

                    var _new = AllProviders.DtUpdate == null && AllProviders.DtInsert < DateTimeOffset.UtcNow.AddDays(-7);
                    var _old = AllProviders.DtUpdate != null && AllProviders.DtUpdate.Value < DateTimeOffset.UtcNow.AddDays(-7);

                    if (_new || _old)
                    {
                        AllProviders.Update();
                        AllProviders.Items = result.OrderBy(o => int.Parse(o.id ?? "0")).ToList();
                        await _repo.Upsert(AllProviders, cancellationToken);
                    }
                }

                return AllProviders;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
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