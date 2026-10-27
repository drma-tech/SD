namespace SD.WEB.Modules
{
    public partial class IndexPage
    {
        private WatchingList? Watching { get; set; }
        private WishList? Wish { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            WatchingApi.DataChanged += async model => { Watching = model; await InvokeAsync(StateHasChanged); };
            WishApi.DataChanged += async model => { Wish = model; await InvokeAsync(StateHasChanged); };
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            Watching = await WatchingApi.Get(states: [], token);
            Wish = await WishApi.Get(states: [], token);
        }
    }
}