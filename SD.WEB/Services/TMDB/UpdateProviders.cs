using SD.Shared.Core;
using SD.Shared.Helper;
using SD.Shared.Modal;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Tmdb;
using SD.WEB.Core;

namespace SD.WEB.Services.TMDB
{
    public class UpdateProviders
    {
        private readonly IConfiguration Configuration;

        public UpdateProviders(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        public async Task UpdateAllProvider(HttpClient http, IStorageService storage)
        {
            var options = Configuration.GetSection(TmdbOptions.Section).Get<TmdbOptions>();
            var result = new List<Provider>();
            var details = await new ProviderServide().GetAllProviders(http, storage.Local);

            foreach (var region in EnumHelper.GetList<Region>())
            {
                var parameter = new Dictionary<string, string>()
                {
                    { "api_key", options.ApiKey },
                    { "language", Language.enUS.GetName(false) },
                    { "watch_region", region.ValueObject.ToString() }
                };

                var movies = await http.Get<TMDB_AllProviders>(storage.Session, options.BaseUri + "watch/providers/movie".ConfigureParameters(parameter));

                AddProvider(result, movies.results, details, (Region)region.ValueObject, MediaType.movie);

                var tvs = await http.Get<TMDB_AllProviders>(storage.Session, options.BaseUri + "watch/providers/tv".ConfigureParameters(parameter));

                AddProvider(result, tvs.results, details, (Region)region.ValueObject, MediaType.tv);
            }

            storage.Session.SetItem("AllProviders", result.OrderBy(o => int.Parse(o.id)));
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