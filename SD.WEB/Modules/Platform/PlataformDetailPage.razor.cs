using Microsoft.AspNetCore.Components;

namespace SD.WEB.Modules.Platform
{
    public partial class PlataformDetailPage
    {
        [Parameter] public string? Id { get; set; }
        private bool isNotFound;

        public RenderControlState<ProviderModel?> State { get; set; } = new(null, obj => obj == null);
        private AllProviders? AllProviders { get; set; }
        public ProviderModel? Provider { get; set; }
        private List<EnumFieldObject<Country>> regions = [];

        public WatchingList? Watching { get; set; }
        public WishList? Wish { get; set; }

        public MyProviders? MyProviders { get; set; }
        public SD.Shared.Enums.Platform? Platform { get; set; }
        public SD.Shared.Enums.Country? SelectedRegion { get; set; }

        private bool ShowMovieFilter => Provider?.types.Contains(MediaType.movie) ?? false;
        private bool ShowTvFilter => Provider?.types.Contains(MediaType.tv) ?? false;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            WatchingApi.DataChanged += model => { Watching = model; StateHasChanged(); };
            WishApi.DataChanged += model => { Wish = model; StateHasChanged(); };
        }

        protected override async Task LoadStaticDataAsync()
        {
            await State.StartLoading.Invoke(null);

            AllProviders = await AllProvidersApi.GetAll(state: null, Cts.Token);
            Provider = AllProviders?.Items.SingleOrDefault(s => string.Equals(s.id, Id, StringComparison.OrdinalIgnoreCase));

            if (Provider == null)
            {
                isNotFound = true;
            }
            else
            {
                regions = [.. EnumHelper.GetList<Country>().Where(p => Provider.regions.Contains(p.Value))];

                if (AppStateStatic.IsPrerendering(Navigation))
                {
                    SelectedRegion = regions[0].Value;
                }
                else
                {
                    var temp = await AppStateStatic.GetRegion(IpInfoApi, JsRuntime, Cts.Token);
                    if (regions.Select(p => p.Value).Contains(temp))
                    {
                        SelectedRegion = temp;
                    }
                }
            }

            await State.FinishLoading.Invoke(Provider);
        }

        protected override async Task<bool> LoadInteropDataAsync(Microsoft.JSInterop.IJSRuntime JsRuntime)
        {
            Platform = await AppStateStatic.GetPlatform(JsRuntime, Cts.Token);

            return true;
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            MyProviders = await MyProvidersApi.Get(states: [], token);
            Watching = await WatchingApi.Get(states: [], token);
            Wish = await WishApi.Get(states: [], token);
        }

        private async Task SelectedRegionChanged(Country? value)
        {
            SelectedRegion = value;

            if (value.HasValue)
            {
                await JsRuntime.Utils().SetStorage("region", value.Value.GetFieldSettings(translate: false).Name, JavascriptContext.Default.String, Cts.Token);

                await AppStateStatic.ChangeRegionAsync(value.Value);
            }
        }

        private Dictionary<string, string> GetExtraParameters(string? providerId, string sortBy)
        {
            ArgumentNullException.ThrowIfNull(providerId);

            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "with_watch_providers", providerId },
                { "sort_by", sortBy },
                { "watch_region", SelectedRegion?.ToString() ?? "NONE" },
            };
        }

        private string? GetSubtitle()
        {
            if (Provider == null) return null;
            var models = string.Join(", ", Provider.models.Select(s => s.GetFieldSettings().Name));
            return $"{Translations.Module.Media.DeliveryModel}: {(models.Empty() ? "null" : models)} | {Translations.Module.Media.Countries}: {Provider.regions.Count}";
        }

        private async Task Add()
        {
            try
            {
                if (!AppStateStatic.IsAuthenticated)
                {
                    await ShowWarning(Translations.Notification.YouMustLogged);
                    return;
                }

                MyProviders ??= new MyProviders(AppStateStatic.UserId);

                var region = await AppStateStatic.GetRegion(IpInfoApi, JsRuntime, Cts.Token);

                var item = new MyProvidersItem { id = Provider?.id, name = Provider?.name, logo = Provider?.logo_path, region = region };
                MyProviders = await MyProvidersApi.Add(MyProviders, item, states: [], AppStateStatic.ActiveProduct, ApiContext.Default.MyProvidersItem, Cts.Token);

                await ShowSuccess(Translations.Notification.PlatformAdded);
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task Remove()
        {
            try
            {
                if (!AppStateStatic.IsAuthenticated)
                {
                    await ShowWarning(Translations.Notification.YouMustLogged);
                    return;
                }

                MyProviders ??= new MyProviders(AppStateStatic.UserId);

                MyProviders = await MyProvidersApi.Remove(MyProviders.Items.First(f => string.Equals(f.id, Provider?.id, StringComparison.OrdinalIgnoreCase)), states: [], ApiContext.Default.MyProvidersItem, Cts.Token);

                await ShowSuccess(Translations.Notification.PlatformRemoved);
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }
    }
}