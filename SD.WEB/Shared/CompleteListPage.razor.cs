using Microsoft.AspNetCore.Components;
using SD.WEB.Api.Module.Cosmos.Anonymous;

namespace SD.WEB.Shared
{
    public partial class CompleteListPage
    {
        [Parameter] public string? id { get; set; }
        [Parameter] public bool OnlyYear { get; set; }
        [Parameter] public string? Type { get; set; }

        public WatchingList? Watching { get; set; }
        public WishList? Wish { get; set; }
        public bool OrderByComments { get; set; }

        public IMediaListApi? MediaListApi { get; set; }
        public bool IsImdb { get; set; }
        public MediaType? TypeSelected { get; set; }
        public IDictionary<string, string> StringParameters { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public bool CommentsIsImage { get; set; }
        public string? TitleHead { get; set; }
        public string? DescriptionHead { get; set; }
        public bool DetectRegions { get; set; }

        private EnumLists? List { get; set; }
        private string Title => TitleHead ?? $"{List?.GetFieldSettings().Name?.CustomFormat(DateTime.Now.Year)}" ?? "Title Error";
        private string Description => DescriptionHead ?? $"{List?.GetFieldSettings().Description}" ?? "Description Error";

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (id.NotEmpty() && Navigation.Uri.Contains("list", StringComparison.OrdinalIgnoreCase)) List = id.ParseToEnum<EnumLists>();
            if (Type.NotEmpty()) TypeSelected = Type.ParseToEnum<MediaType>();

            WatchingApi.DataChanged += model => { Watching = model; StateHasChanged(); };
            WishApi.DataChanged += model => { Wish = model; StateHasChanged(); };
        }

        protected override async Task LoadStaticDataAsync()
        {
            await ProcessNonList();

            MediaListApi ??= new TmdbListApi(factory);
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            Watching = await WatchingApi.Get(actions: null, token);
            Wish = await WishApi.Get(actions: null, token);
        }

        private async Task ProcessNonList()
        {
            if (Navigation.Uri.Contains("platform", StringComparison.OrdinalIgnoreCase))
            {
                var providers = await AllProvidersApi.GetAll(state: null, Cts.Token);
                var provider = providers?.Items.SingleOrDefault(s => string.Equals(s.id, id, StringComparison.OrdinalIgnoreCase));

                if (Navigation.Uri.Contains("popular", StringComparison.OrdinalIgnoreCase))
                {
                    TitleHead = $"{provider?.name}: {Translations.Module.Media.Popular} ({TypeSelected?.GetFieldSettings().Name})";
                    DescriptionHead = Translations.Module.Media.PopularDesc.CustomFormat(provider?.name);
                    StringParameters = GetExtraParameters(id, "popularity.desc");
                }
                else if (Navigation.Uri.Contains("new", StringComparison.OrdinalIgnoreCase))
                {
                    TitleHead = $"{provider?.name}: {Translations.Module.Media.Release} ({TypeSelected?.GetFieldSettings().Name})";
                    DescriptionHead = Translations.Module.Media.NewsDesc.CustomFormat(provider?.name);
                    DetectRegions = true;
                    StringParameters = GetExtraParameters(id, "primary_release_date.desc");
                }
                else if (Navigation.Uri.Contains("top", StringComparison.OrdinalIgnoreCase))
                {
                    TitleHead = $"{provider?.name}: {Translations.Module.Media.TopRated} ({TypeSelected?.GetFieldSettings().Name})";
                    DescriptionHead = Translations.Module.Media.RecommendedDesc.CustomFormat(provider?.name);
                    DetectRegions = true;
                    StringParameters = GetExtraParameters(id, "vote_average.desc");
                }

                MediaListApi = new TmdbDiscoveryApi(factory);
            }
        }

        private static Dictionary<string, string> GetExtraParameters(string? providerId, string sortBy)
        {
            ArgumentNullException.ThrowIfNull(providerId);

            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "with_watch_providers", providerId },
                { "sort_by", sortBy },
            };
        }
    }
}