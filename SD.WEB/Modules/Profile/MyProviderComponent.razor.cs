using Microsoft.AspNetCore.Components;

namespace SD.WEB.Modules.Profile
{
    public partial class MyProviderComponent
    {
        [Parameter][EditorRequired] public WatchingList? WatchingList { get; set; }
        [Parameter][EditorRequired] public WishList? WishList { get; set; }
        [Parameter][EditorRequired] public string? Culture { get; set; }

        [Parameter] public RenderControlState<MyProviders?> State { get; set; } = new(null, obj => obj == null || obj.Items.Empty());
        private AllProviders? AllProviders { get; set; }
        private bool ProvidersChanged { get; set; }

        public Country Region { get; set; } = Country.US;

        protected override void OnInitialized()
        {
            State.CustomMessageWarning = Translations.Module.Profile.MarkPlatforms;

            MyProvidersApi.DataChanged += model =>
            {
                _ = State.FinishLoading.Invoke(model);
            };
        }

        protected override async Task LoadStaticDataAsync()
        {
            AllProviders = await AllProvidersApi.GetAll(state: null, Cts.Token);
        }

        protected override async Task<bool> LoadInteropDataAsync(Microsoft.JSInterop.IJSRuntime JsRuntime)
        {
            Region = await AppStateStatic.GetRegion(IpInfoApi, JsRuntime, Cts.Token);

            return true;
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            foreach (var item in State.Instance?.Items ?? new HashSet<MyProvidersItem>())
            {
                var provider = AllProviders?.Items.FirstOrDefault(f => string.Equals(f.id, item.id, StringComparison.OrdinalIgnoreCase));

                if (!string.Equals(provider?.name, item.name, StringComparison.OrdinalIgnoreCase) || !string.Equals(provider?.logo_path, item.logo, StringComparison.OrdinalIgnoreCase))
                {
                    ProvidersChanged = true;
                }
            }

            if (State.Instance != null && State.Instance.Items.Any(a => !a.region.HasValue)) //update region (legacy)
            {
                foreach (var item in State.Instance.Items)
                {
                    if (!item.region.HasValue)
                    {
                        var provider = AllProviders?.Items.FirstOrDefault(f => string.Equals(f.id, item.id, StringComparison.OrdinalIgnoreCase));

                        if (provider != null)
                        {
                            item.region = provider.regions.Contains(Region) ? Region : provider.regions.FirstOrDefault();
                        }
                    }
                }

                await ShowInfo(Translations.Module.Profile.RegionsApplied);
                await MyProvidersApi.Update(State.Instance, [State], AppStateStatic.ActiveProduct, validatePlan: false, token);

                StateHasChanged();
            }
        }

        public async Task ShowProviderPopup(MyProvidersItem item)
        {
            AllProviders ??= await AllProvidersApi.GetAll(state: null, Cts.Token);

            await DialogService.PlatformPopup(AllProviders?.Items.FirstOrDefault(f => string.Equals(f.id, item.id, StringComparison.OrdinalIgnoreCase)), WatchingList, WishList, item.region?.ToString(), item.id, Culture);
        }

        private async Task UpdateProviders()
        {
            foreach (var item in State.Instance?.Items ?? new HashSet<MyProvidersItem>())
            {
                var provider = AllProviders?.Items.FirstOrDefault(f => string.Equals(f.id, item.id, StringComparison.OrdinalIgnoreCase));

                if (provider == null) continue;
                item.name = provider.name;
                item.logo = provider.logo_path;
            }

            await MyProvidersApi.Update(State.Instance, [State], product: null, validatePlan: false, Cts.Token);
            ProvidersChanged = false;
        }
    }
}