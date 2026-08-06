using HtmlAgilityPack;
using MudBlazor;
using SD.Shared.Models.List.Tmdb;
using System.Net.Http.Json;

namespace SD.WEB.Modules.Platform
{
    public partial class PlatformEdit
    {
        public MediaType MediaType { get; set; } = MediaType.movie;
        public DeliveryModel? DeliveryModel { get; set; }

        private AllProviders? AllProviders { get; set; }
        public IEnumerable<EnumFieldObject<DeliveryModel>> DeliveryModels { get; set; } = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();
            DeliveryModels = EnumHelper.GetList<DeliveryModel>();
        }

        protected override async Task LoadStaticDataAsync()
        {
            AllProviders = await AllProvidersApi.GetAll(actions: null, Cts.Token);
        }

        protected async Task SaveSession()
        {
            if (AllProviders != null)
            {
                await JsRuntime.Utils().SetStorage("AllProviders", AllProviders, JavascriptContext.Default.AllProviders, Cts.Token);
            }
        }

        protected async Task SyncProvidersOnClick()
        {
            try
            {
                var result = new List<ProviderModel>();

                if (AllProviders != null)
                {
                    var details = AllProviders.Items;
                    var blocked_providers = new List<string>() { "Amazon Channel", "Amzon Channel", "Apple TV Channel", "Roku Premium Channel", "on U-Next" };

                    foreach (var region in EnumHelper.GetValues<Country>())
                    {
                        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            { "api_key", TmdbOptions.ApiKey },
                            { "language", ContentLanguage.enUS.GetFieldSettings(translate: false).Name ?? "en-US" },
                            { "watch_region", region.ToString().ToUpperInvariant() },
                        };

                        using var http = new HttpClient();
                        var movies = await http.GetFromJsonAsync(TmdbOptions.BaseUri + "watch/providers/movie".ConfigureParameters(parameter), JavascriptContext.Default.TMDB_AllProviders, Cts.Token);
                        movies?.results.RemoveAll(r => blocked_providers.Exists(k => r.provider_name.NotEmpty() && r.provider_name.Contains(k, StringComparison.InvariantCultureIgnoreCase)));
                        if (movies != null) AddProvider(result, movies.results, details, region, MediaType.movie);

                        var tvs = await http.GetFromJsonAsync(TmdbOptions.BaseUri + "watch/providers/tv".ConfigureParameters(parameter), JavascriptContext.Default.TMDB_AllProviders, Cts.Token);
                        tvs?.results.RemoveAll(r => blocked_providers.Exists(k => r.provider_name.NotEmpty() && r.provider_name.Contains(k, StringComparison.InvariantCultureIgnoreCase)));
                        if (tvs != null) AddProvider(result, tvs.results, details, region, MediaType.tv);
                    }

                    AllProviders.Items = [.. result.OrderBy(o => int.Parse(o.id ?? "0", System.Globalization.CultureInfo.InvariantCulture))];

                    await SaveSession();
                }

                await ShowSuccess("Synchronization Finished");
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private static void AddProvider(List<ProviderModel> final_list, IEnumerable<ProviderBase> new_providers, IEnumerable<ProviderModel> current_providers, Country region, MediaType type)
        {
            foreach (var item in new_providers)
            {
                var detail = current_providers.FirstOrDefault(f => string.Equals(f.id, item.provider_id.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.Ordinal));
                var new_item = final_list.FirstOrDefault(f => string.Equals(f.id, item.provider_id.ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.Ordinal));

                if (new_item == null)
                {
                    final_list.Add(new ProviderModel
                    {
                        //api
                        id = item.provider_id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        name = item.provider_name,
                        priority = item.display_priority,
                        logo_path = item.logo_path,
                        //own data (manual update)
                        description = detail?.description,
                        link = detail?.link,
                        models = detail?.models ?? [],
                        //api by regions
                        regions = [region],
                        types = [type],
                    });
                }
                else
                {
                    if (!new_item.regions.Contains(region))
                    {
                        new_item.regions.Add(region);
                    }
                    new_item.regions = [.. new_item.regions.Order()];

                    if (!new_item.types.Contains(type))
                    {
                        new_item.types.Add(type);
                    }
                    new_item.types = [.. new_item.types.Order()];
                }
            }
        }

        protected Task<DataGridEditFormAction> RowUpdated(ProviderModel model)
        {
            var item = AllProviders?.Items.FirstOrDefault(f => f.id == model.id);

            item = model;

            return Task.FromResult(DataGridEditFormAction.Close);
        }

        protected async Task<string> GetDescriptionOnClick(string? url)
        {
            ArgumentNullException.ThrowIfNull(url);

            try
            {
                var client = new HttpClient();

                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://cors-anywhere.herokuapp.com/" + url);
                requestMessage.Headers.Add("origin", "x-requested-with");

                var response = await client.SendAsync(requestMessage, Cts.Token);
                using HttpContent content = response.Content;
                var sourcedata = await content.ReadAsStringAsync(Cts.Token);

                if (string.Equals(sourcedata, "See /corsdemo for more info\n", StringComparison.OrdinalIgnoreCase))
                {
                    return "https://cors-anywhere.herokuapp.com";
                }

                var result = GetMetaTagValue(sourcedata);
                return result;
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
                return ex.Message;
            }
        }

        private static string GetMetaTagValue(string html)
        {
            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            var metaTags = doc.DocumentNode.SelectNodes("//meta");
            if (metaTags != null)
            {
                foreach (var sitetag in metaTags)
                {
                    if (sitetag.Attributes["name"] != null && sitetag.Attributes["content"] != null && sitetag.Attributes["name"].Value.Contains("description", StringComparison.OrdinalIgnoreCase))
                    {
                        var content = sitetag.Attributes["content"].Value;

                        if (string.IsNullOrEmpty(content)) return "description empty";

                        return System.Web.HttpUtility.HtmlDecode(content);
                    }
                }
            }
            else
            {
                return "something wrong (metaTags null)";
            }

            return "no description found";
        }
    }
}