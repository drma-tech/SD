using Microsoft.AspNetCore.Components.Web;

namespace SD.WEB.Modules
{
    public partial class Index
    {
        private WatchingList? Watching { get; set; }
        private WishList? Wish { get; set; }

        public IEnumerable<EnumFieldObject<MediaType>> Types { get; set; } = [];
        public IEnumerable<EnumFieldObject<MovieGenre>> MovieGenres { get; set; } = [];
        public IEnumerable<EnumFieldObject<TvGenre>> TvGenres { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            WatchingApi.DataChanged += model => { Watching = model; StateHasChanged(); };
            WishApi.DataChanged += model => { Wish = model; StateHasChanged(); };

            Types = EnumHelper.GetList<MediaType>().Where(p => p.Value != MediaType.person);
            MovieGenres = EnumHelper.GetList<MovieGenre>();
            TvGenres = EnumHelper.GetList<TvGenre>();
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            Watching = await WatchingApi.Get(actions: null, token);
            Wish = await WishApi.Get(actions: null, token);
        }

        private void KeyPress(KeyboardEventArgs args)
        {
            if (AppStateStatic.Query.Empty()) return;

            if (string.Equals(args.Key, "Enter", StringComparison.OrdinalIgnoreCase))
            {
                Navigation.NavigateTo($"/{Culture}/search");
            }
        }

        private async Task KeyPressKeyboard(KeyboardEventArgs args)
        {
            if (AppStateStatic.Query.Empty()) return;

            if (string.Equals(args.Key, "Enter", StringComparison.OrdinalIgnoreCase))
            {
                await SearchKeyWords();
            }
        }

        private int _currentPage = 1;
        public bool DisableLoadMore { get; set; }

        private async Task SearchKeyWords()
        {
            AppStateStatic.Keywords = [];
            _currentPage = 1;
            Dictionary<string, string> parameters = new(StringComparer.OrdinalIgnoreCase) { { "query", AppStateStatic.Query ?? "" }, { "sort_by", "popularity.desc" } };

            var (list, lastPage) = await TmdbSearchKeywordApi.GetKeywords(AppStateStatic.Keywords, parameters, _currentPage++, Cts.Token);
            AppStateStatic.Keywords = list;

            DisableLoadMore = lastPage || AppStateStatic.Keywords.Count >= 100;
        }

        private async Task LoadMore()
        {
            Dictionary<string, string> parameters = new(StringComparer.OrdinalIgnoreCase) { { "query", AppStateStatic.Query ?? "" }, { "sort_by", "popularity.desc" } };

            var (list, lastPage) = await TmdbSearchKeywordApi.GetKeywords(AppStateStatic.Keywords, parameters, _currentPage++, Cts.Token);
            AppStateStatic.Keywords = list;
            DisableLoadMore = lastPage || AppStateStatic.Keywords.Count >= 100;
        }
    }
}