using Microsoft.AspNetCore.Components;

namespace SD.WEB.Modules.Platform
{
    public partial class PlataformIndex
    {
        public RenderControlState<AllProviders> Actions { get; set; } = new(obj => obj == null || obj.Items.Empty());
        private AllProviders? AllProviders { get; set; }

        private MyProviders? MyProviders { get; set; }

        public DeliveryModel? DeliveryModel { get; set; }
        public MediaType? MediaType { get; set; }
        private string? name;

        [Parameter] public string? Region { get; set; }
        private bool isNotFound;

        public Country? RegionEnum { get; set; }
        private bool ShowAll { get; set; }

        public IEnumerable<ProviderModel> GetFilteredProviders(bool all)
        {
            return AllProviders?.Items
                .Where(p => RegionEnum.HasValue && p.regions.Contains(RegionEnum.Value) 
                    && (p.models.Empty() || p.models.Any(a => DeliveryModel == null || a == DeliveryModel)) 
                    && (p.types.Empty() || p.types.Any(a => MediaType == null || a == MediaType)) 
                    && (name == null || p.name!.Contains(name, StringComparison.InvariantCultureIgnoreCase)))
                .OrderBy(o => o.priority)
                .Take(all ? 1000 : (Region.Empty() ? 45 : 20))
                ?? [];
        }

        public IEnumerable<EnumFieldObject<DeliveryModel>> DeliveryModels { get; set; } = [];
        public IEnumerable<EnumFieldObject<MediaType>> MediaTypes { get; set; } = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AppStateStatic.RegionChanged.Subscribe(async region => { RegionEnum = region; await InvokeAsync(StateHasChanged); }, Cts.Token);
        }

        protected override List<string?> GetParameterKey()
        {
            return [
                Culture,
                Region,
            ];
        }

        protected override async Task LoadParameterDataAsync()
        {
            DeliveryModels = EnumHelper.GetList<DeliveryModel>();
            MediaTypes = EnumHelper.GetList<MediaType>().Where(p => p.Value != SD.Shared.Enums.MediaType.person);

            try
            {
                RegionEnum = Region.NotEmpty() ? Region.ParseToEnum<Country>() : await AppStateStatic.GetRegion(IpInfoApi, JsRuntime, Cts.Token);
            }
            catch (Exception)
            {
                isNotFound = true;
            }

            AllProviders = await AllProvidersApi.GetAll(Actions, Cts.Token);
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            MyProviders = await MyProvidersApi.Get(actions: null, token);
        }

        private async Task Add(ProviderModel provider)
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

                var item = new MyProvidersItem { id = provider?.id, name = provider?.name, logo = provider?.logo_path, region = region };
                MyProviders = await MyProvidersApi.Add(MyProviders, item, AppStateStatic.ActiveProduct, ApiContext.Default.MyProvidersItem, Cts.Token);

                await ShowSuccess(Translations.Notification.PlatformAdded);
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task Remove(ProviderModel provider)
        {
            try
            {
                if (!AppStateStatic.IsAuthenticated)
                {
                    await ShowWarning(Translations.Notification.YouMustLogged);
                    return;
                }

                MyProviders ??= new MyProviders(AppStateStatic.UserId);

                MyProviders = await MyProvidersApi.Remove(MyProviders.Items.First(f => string.Equals(f.id, provider?.id, StringComparison.OrdinalIgnoreCase)), ApiContext.Default.MyProvidersItem, Cts.Token);

                await ShowSuccess(Translations.Notification.PlatformRemoved);
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task ShareClick(ProviderModel provider)
        {
            await JsRuntime.Utils().Share(provider.name, provider.description, $"/{Culture}/platform/{provider.id}", CancellationToken.None);
        }
    }
}