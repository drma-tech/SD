namespace SD.WEB.Modules.Profile
{
    public partial class ProfilePage
    {
        public RenderControlState<MyProviders> ProviderActions { get; set; } = new(obj => obj == null || obj.Items.Empty());

        public RenderControlState<WishList> WishActionsMovie { get; set; } = new(list => list == null || list.Movies.Empty());
        public RenderControlState<WishList> WishActionsTv { get; set; } = new(list => list == null || list.Shows.Empty());

        public RenderControlState<WatchingList> WatchingActionsMovie { get; set; } = new(list => list == null || list.Movies.Empty());
        public RenderControlState<WatchingList> WatchingActionsTv { get; set; } = new(list => list == null || list.Shows.Empty());

        private MyProviders? MyProviders { get; set; }
        public WatchingList? Watching { get; set; }
        public WishList? Wish { get; set; }

        public bool Reviewed { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            WatchingApi.DataChanged += model =>
            {
                _ = WatchingActionsMovie.StartLoading.Invoke(null);
                _ = WatchingActionsTv.StartLoading.Invoke(null);
                Watching = model;
                _ = WatchingActionsMovie.FinishLoading.Invoke(model);
                _ = WatchingActionsTv.FinishLoading.Invoke(model);
                StateHasChanged();
            };
            WishApi.DataChanged += model =>
            {
                _ = WishActionsMovie.StartLoading.Invoke(null);
                _ = WishActionsTv.StartLoading.Invoke(null);
                Wish = model;
                _ = WishActionsMovie.FinishLoading.Invoke(model);
                _ = WishActionsTv.FinishLoading.Invoke(model);
                StateHasChanged();
            };
        }

        protected override async Task<bool> LoadInteropDataAsync(Microsoft.JSInterop.IJSRuntime JsRuntime)
        {
            Reviewed = await JsRuntime.Utils().GetStorage("reviewed", JavascriptContext.Default.Boolean, Cts.Token);

            if (AppStateStatic.IsAuthenticated)
            {
                var subscriptionPopup = await JsRuntime.Utils().GetStorage("subscription-popup", JavascriptContext.Default.Boolean, Cts.Token);

                if (!subscriptionPopup)
                {
                    await DialogService.SubscriptionPopup();
                    await JsRuntime.Utils().SetStorage("subscription-popup", value: true, JavascriptContext.Default.Boolean, Cts.Token);
                }
            }

            return true;
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            var demo = false;

            await WishActionsMovie.StartLoading.Invoke(null);
            await WishActionsTv.StartLoading.Invoke(null);
            await WatchingActionsMovie.StartLoading.Invoke(null);
            await WatchingActionsTv.StartLoading.Invoke(null);

            Wish = await WishApi.Get(actions: null, token);
            Watching = await WatchingApi.Get(actions: null, token);

            if ((Wish == null || Wish.Items(MediaType.movie).Empty()) && AppStateStatic.IsAuthenticated)
            {
                Wish ??= new WishList(AppStateStatic.UserId);

                var (list, _) = await TmdbListApi.GetList([], actions: null, MediaType.movie, stringParameters: null, EnumLists.CertifiedStreamingDiscoveryMovies, cancellationToken: token);
                var media = list.FirstOrDefault();

                if (media != null)
                {
                    var item = new WishListItem(media.tmdb_id, media.title, media.poster_small, media.runtime);

                    Wish = await WishApi.Add(MediaType.movie, Wish, item, product: null, Cts.Token);

                    demo = true;
                }
            }

            if (Watching == null && AppStateStatic.IsAuthenticated)
            {
                //todo: generate demo
            }

            await WishActionsMovie.FinishLoading.Invoke(Wish);
            await WishActionsTv.FinishLoading.Invoke(Wish);
            await WatchingActionsMovie.FinishLoading.Invoke(Watching);
            await WatchingActionsTv.FinishLoading.Invoke(Watching);

            MyProviders = await MyProvidersApi.Get(actions: null, token);

            if (MyProviders == null && AppStateStatic.IsAuthenticated)
            {
                await ProviderActions.StartLoading.Invoke(null);

                MyProviders ??= new MyProviders(AppStateStatic.UserId);

                var AllProviders = await AllProvidersApi.GetAll(actions: null, Cts.Token);
                var region = await AppStateStatic.GetRegion(IpInfoApi, JsRuntime, Cts.Token);

                var provider = AllProviders?.Items.Where(p => p.regions.Contains(region)).OrderBy(o => o.priority).FirstOrDefault();

                var item = new MyProvidersItem { id = provider?.id, name = provider?.name, logo = provider?.logo_path, region = region };
                MyProviders = await MyProvidersApi.Add(MyProviders, item, AppStateStatic.ActiveProduct, ApiContext.Default.MyProvidersItem, Cts.Token);

                demo = true;

                await ProviderActions.FinishLoading.Invoke(MyProviders);
            }

            if (demo)
            {
                await ShowInfo(Translations.Module.Profile.DemoAlert);
            }
        }

        // private async Task SetReviewed()
        // {
        //     await JsRuntime.InvokeAsync<string>("SetLocalStorage", "reviewed", "true");
        //     Reviewed = "true";
        // }
    }
}
