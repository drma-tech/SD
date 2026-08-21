namespace SD.WEB.Modules
{
    public partial class IndexPage
    {
        private WatchingList? Watching { get; set; }
        private WishList? Wish { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            WatchingApi.DataChanged += model => { Watching = model; StateHasChanged(); };
            WishApi.DataChanged += model => { Wish = model; StateHasChanged(); };
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            Watching = await WatchingApi.Get(actions: null, token);
            Wish = await WishApi.Get(actions: null, token);
        }
    }
}