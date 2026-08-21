using Microsoft.AspNetCore.Components;
using SD.Shared.Models.List.Tmdb;
using System.Globalization;

namespace SD.WEB.Shared
{
    public partial class MediaListFull
    {
        [Parameter][EditorRequired] public WatchingList? Watching { get; set; }
        [Parameter][EditorRequired] public WishList? Wish { get; set; }
        [Parameter][EditorRequired] public string? Culture { get; set; }

        [Parameter] public bool ShowHead { get; set; } = true;
        [Parameter] public string? TitleHead { get; set; }
        [Parameter] public string? SubtitleHead { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public string? Icon { get; set; }
        [Parameter] public bool FullPage { get; set; } = false;
        [Parameter] public bool Popup { get; set; } = false;
        [Parameter] public bool OnlyYear { get; set; }

        [Parameter] public RenderControlState<ISet<MediaDetail>> State { get; set; } = new(list => list == null || list.Empty());
        [Parameter] public ISet<MediaDetail> Items { get; set; } = new HashSet<MediaDetail>();
        [Parameter] public EventCallback<ISet<MediaDetail>> ItemsChanged { get; set; }

        [Parameter] public bool IsImdb { get; set; }
        [Parameter] public IMediaListApi? MediaListApi { get; set; }
        [Parameter] public MediaType? TypeSelected { get; set; }
        [Parameter] public IDictionary<string, string> StringParameters { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        [Parameter] public EnumLists? List { get; set; }
        [Parameter] public string? CommentsSeparator { get; set; } = ",";
        [Parameter] public bool CommentsIsImage { get; set; }
        [Parameter] public bool OrderByComments { get; set; }

        [Parameter] public bool DetectRegions { get; set; }
        [Parameter] public int MinQtdItems { get; set; } = 30;

        private string? GetTitle => List != null ? List?.GetFieldSettings().Name : TitleHead;
        public bool DisableLoadMore { get; set; }
        private int _currentPage = 1;

        protected override void OnInitialized()
        {
            //if (!string.IsNullOrEmpty(list_id) && !NextPage.HasDelegate) throw new ArgumentNullException(nameof(NextPage));
            //if (NextPage.HasDelegate && string.IsNullOrEmpty(list_id)) throw new ArgumentNullException(nameof(list_id));

            //TODO: analyse this
            // AppState.WishListChanged += StateHasChanged;
            // AppState.WatchedListChanged += StateHasChanged;

            if (DetectRegions) AppStateStatic.RegionChanged.Subscribe(async (Region) => await LoadParameterDataAsync(), Cts.Token);
        }

        protected override List<string?> GetParameterKey()
        {
            return
            [
                MediaListApi?.GetType().FullName,
                GetDictionaryKey(StringParameters),
                GetCollectionKey(Items.Select(s => s.tmdb_id)),
                TypeSelected.ToString(),
            ];
        }

        protected override async Task LoadParameterDataAsync()
        {
            await State.StartLoading.Invoke(null);
            await LoadItems();
            await State.FinishLoading.Invoke(Items);
        }

        private async Task LoadItems()
        {
            if (MediaListApi != null)
            {
                var (_, lastPage) = await MediaListApi.GetList(Items, State, TypeSelected, StringParameters, List, 1, Cts.Token);

                DisableLoadMore = lastPage || Items.Count >= 200;

                if (Items.Count < MinQtdItems) //force reload, if the filters bring few records
                {
                    _ = await MediaListApi.GetList(Items, State, TypeSelected, StringParameters, List, ++_currentPage, Cts.Token);

                    DisableLoadMore = lastPage || Items.Count >= 200;
                }

                if (OrderByComments) Items = Items.OrderByDescending(o => int.Parse(o.comments?.Split(",")[^1] ?? "0", System.Globalization.CultureInfo.InvariantCulture)).ToHashSet();

                await ItemsChanged.InvokeAsync(Items);
            }
        }

        private async Task OpenPopupMedia(MediaDetail? media)
        {
            string? tmdbId;

            if (IsImdb) //for now, only tv series (imdb) need this kind of workaround
            {
                tmdbId = await ExternalIdApi.GetTmdbId(media?.MediaType, media?.tmdb_id, Cts.Token);
            }
            else
            {
                tmdbId = media?.tmdb_id;
            }

            if (tmdbId.Empty())
            {
                await ShowError("Unable to display this content. Please try again later.");
                return;
            }

            if (media?.MediaType == MediaType.person)
            {
                var result = await TmdbCreditApi.GetListByPerson(tmdbId, Cts.Token);
                var items = new HashSet<MediaDetail>();

                foreach (var item in result?.crew ?? Enumerable.Empty<CrewByPerson>())
                {
                    var type = string.Equals(item.media_type, "tv", StringComparison.OrdinalIgnoreCase) ? MediaType.tv : MediaType.movie;
                    items.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
                        title = type == MediaType.movie ? item.title : item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = type == MediaType.movie ? item.release_date?.GetDate() : item.first_air_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average ?? 0 : 0,
                        MediaType = type,
                    });
                }

                foreach (var item in result?.cast ?? [])
                {
                    var type = string.Equals(item.media_type, "tv", StringComparison.OrdinalIgnoreCase) ? MediaType.tv : MediaType.movie;

                    if (type == MediaType.movie && item.order > 24) continue;
                    if (type == MediaType.tv && item.episode_count < 3) continue;

                    items.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
                        title = type == MediaType.movie ? item.title : item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = type == MediaType.movie ? item.release_date?.GetDate() : item.first_air_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average ?? 0 : 0,
                        MediaType = type,
                        comments = type == MediaType.tv ? string.Create(CultureInfo.InvariantCulture, $"{item.episode_count} episodes") : "",
                    });
                }

                await DialogService.CompleteListPopup($"{media.title}", Watching, Wish, items.OrderByDescending(o => o.release_date).ToHashSet(), Culture);
            }
            else
            {
                if (Popup)
                {
                    await DialogService.MediaPopup(Watching, Wish, media?.MediaType, tmdbId, Culture);
                }
                else
                {
                    Navigation.NavigateTo($"/{Culture}/media/{media?.MediaType.ToString()}/{tmdbId}");
                }
            }
        }

        private async Task LoadMore()
        {
            if (MediaListApi != null)
            {
                var (_, lastPage) = await MediaListApi.GetList(Items, State, TypeSelected, StringParameters, List, ++_currentPage, Cts.Token);

                DisableLoadMore = lastPage || Items.Count >= 250;

                if (OrderByComments) Items = Items.OrderByDescending(o => int.Parse(o.comments?.Split(",")[^1] ?? "0", System.Globalization.CultureInfo.InvariantCulture)).ToHashSet();

                await ItemsChanged.InvokeAsync(Items);
            }
        }
    }
}