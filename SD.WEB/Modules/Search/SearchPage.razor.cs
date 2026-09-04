using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;

namespace SD.WEB.Modules.Search
{
    public partial class SearchPage
    {
        [Parameter][SupplyParameterFromQuery(Name = "keyword")] public int? keyword { get; set; }
        [Parameter][SupplyParameterFromQuery(Name = "index")] public int? index { get; set; }

        public WatchingList? Watching { get; set; }
        public WishList? Wish { get; set; }
        public bool firstAccess { get; set; } = true;

        private HashSet<MediaDetail> Items { get; set; } = [];
        private RenderControlState<ISet<MediaDetail>> State { get; } = new(new HashSet<MediaDetail>(), list => list.Empty());
        private static Dictionary<string, string> ParametersQuery { get; set; } = new(StringComparer.OrdinalIgnoreCase) { { "query", AppStateStatic.Query ?? "" } };
        private static Dictionary<string, string> ParametersKeyword { get; set; } = new(StringComparer.OrdinalIgnoreCase) { { "sort_by", "popularity.desc" }, { "watch_region", "none" } };
        private static Dictionary<string, string> ParametersAdvanced { get; set; } = new(StringComparer.OrdinalIgnoreCase) { { "sort_by", AppStateStatic.SortBy }, { "watch_region", "none" } };

        public IEnumerable<EnumFieldObject<MediaType>> Types { get; set; } = [];
        public IEnumerable<EnumFieldObject<MovieGenre>> MovieGenres { get; set; } = [];
        public IEnumerable<EnumFieldObject<TvGenre>> TvGenres { get; set; } = [];

        protected override void OnInitialized()
        {
            State.CustomMessageWarning = Translations.Module.Landing.SearchReturnedNothing;

            WatchingApi.DataChanged += model => { Watching = model; StateHasChanged(); };
            WishApi.DataChanged += model => { Wish = model; StateHasChanged(); };

            Types = EnumHelper.GetList<MediaType>().Where(p => p.Value != MediaType.person);
            MovieGenres = EnumHelper.GetList<MovieGenre>();
            TvGenres = EnumHelper.GetList<TvGenre>();

            base.OnInitialized();
        }

        protected override async Task LoadStaticDataAsync()
        {
            if (index != null && keyword != null && firstAccess)
            {
                AppStateStatic.Index = index.Value;
                AppStateStatic.Keyword = keyword;
                firstAccess = false;
            }

            if (AppStateStatic.Index == 0)
            {
                ParametersQuery["query"] = AppStateStatic.Query ?? throw new NotificationException("query not present");
            }
            else if (AppStateStatic.Index == 1)
            {
                ParametersKeyword["with_keywords"] = AppStateStatic.Keyword?.ToString(CultureInfo.InvariantCulture) ?? throw new NotificationException("keyword not present");
            }
            else if (AppStateStatic.Index == 2)
            {
                if (AppStateStatic.Type == MediaType.movie)
                {
                    ParametersAdvanced["with_genres"] = ((int)AppStateStatic.MovieGenre!.Value).ToString(CultureInfo.InvariantCulture);
                }
                else if (AppStateStatic.Type == MediaType.tv)
                {
                    ParametersAdvanced["with_genres"] = ((int)AppStateStatic.TvGenre!.Value).ToString(CultureInfo.InvariantCulture);
                }
            }

            await LoadItems();
        }

        private async Task LoadItems(int? newKeyword = null)
        {
            await State.StartLoading.Invoke(null);
            Items.Clear();

            if (AppStateStatic.Index == 0)
            {
                _ = await TmdbSearch.GetList(Items, [State], type: null, ParametersQuery, cancellationToken: Cts.Token);
            }
            else if (AppStateStatic.Index == 1)
            {
                if (newKeyword != null) AppStateStatic.Keyword = newKeyword;
                ParametersKeyword["with_keywords"] = AppStateStatic.Keyword?.ToString(CultureInfo.InvariantCulture) ?? throw new NotificationException("keyword not present");
                _ = await TmdbDiscoveryApi.GetList(Items, [State], type: null, ParametersKeyword, cancellationToken: Cts.Token);
            }
            else if (AppStateStatic.Index == 2)
            {
                _ = await TmdbDiscoveryApi.GetList(Items, [State], AppStateStatic.Type, ParametersAdvanced, cancellationToken: Cts.Token);
            }

            await State.FinishLoading.Invoke(Items);

            if (Items.Empty())
            {
                await ShowWarning(Translations.Module.Landing.SearchReturnedNothing);
            }
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            Watching = await WatchingApi.Get(states: [], token);
            Wish = await WishApi.Get(states: [], token);
        }

        private async Task KeyPress(KeyboardEventArgs args)
        {
            if (AppStateStatic.Query.Empty()) return;

            if (string.Equals(args.Key, "Enter", StringComparison.OrdinalIgnoreCase))
            {
                await OnParametersSetAsync();
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
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "query", AppStateStatic.Query ?? "" } };

            var (list, lastPage) = await TmdbSearchKeywordApi.GetKeywords(AppStateStatic.Keywords, parameters, _currentPage++, Cts.Token);
            AppStateStatic.Keywords = list;

            DisableLoadMore = lastPage || AppStateStatic.Keywords.Count >= 100;
        }

        private async Task LoadMore()
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "query", AppStateStatic.Query ?? "" }, { "sort_by", "popularity.desc" } };

            var (list, lastPage) = await TmdbSearchKeywordApi.GetKeywords(AppStateStatic.Keywords, parameters, _currentPage++, Cts.Token);
            AppStateStatic.Keywords = list;
            DisableLoadMore = lastPage || AppStateStatic.Keywords.Count >= 100;
        }
    }
}