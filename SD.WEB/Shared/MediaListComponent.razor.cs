using Microsoft.AspNetCore.Components;
using MudBlazor;
using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Collections.Interface;
using System.Globalization;

namespace SD.WEB.Shared
{
    public partial class MediaListComponent
    {
        [Parameter][EditorRequired] public WatchingList? Watching { get; set; }
        [Parameter][EditorRequired] public WishList? Wish { get; set; }
        [Parameter][EditorRequired] public string? Culture { get; set; }

        [Parameter] public bool ContainsHeader { get; set; } = false;
        [Parameter] public string? TitleHead { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public string? Icon { get; set; }
        [Parameter] public string? Image { get; set; }
        [Parameter] public bool ShowMovieFilter { get; set; }
        [Parameter] public bool ShowTvFilter { get; set; }
        [Parameter] public bool OnlyYear { get; set; }
        [Parameter] public bool OrderByComments { get; set; }
        [Parameter] public bool CommentsIsImage { get; set; }
        [Parameter] public bool Popup { get; set; } = false;
        [Parameter] public string? CustomExpand { get; set; }
        [Parameter] public Typo? ForceTypoTitle { get; set; }

        [Parameter] public IMediaListApi? MediaListApi { get; set; }
        [Parameter] public EnumLists? List { get; set; }
        [Parameter] public IDictionary<string, string> StringParameters { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        [Parameter] public bool IsImdb { get; set; }
        [Parameter] public int MinQtdItems { get; set; } = 10;

        [Parameter] public bool DetectRegions { get; set; }

        [Parameter] public MediaType TypeSelected { get; set; }

        private string Title => TitleHead ?? List?.GetFieldSettings().Name ?? "Title Error";

        public RenderControlState<ISet<MediaDetail>> State { get; set; } = new(list => list != null && list.Empty());
        public ISet<MediaDetail> Items { get; set; } = new HashSet<MediaDetail>();

        protected override async Task OnInitializedAsync()
        {
            if (ContainsHeader)
            {
                TypeSelected = ShowMovieFilter ? MediaType.movie : MediaType.tv;
            }

            await base.OnInitializedAsync();

            try
            {
                if (MediaListApi == null) throw new NotificationException("MediaListApi is required.");

                if (DetectRegions) AppStateStatic.RegionChanged.Subscribe(async region => await LoadParameterDataAsync(), Cts.Token);
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        protected override List<string?> GetParameterKey()
        {
            return [TypeSelected.ToString(), Culture];
        }

        protected override async Task LoadParameterDataAsync()
        {
            await LoadItems(TypeSelected);
        }

        private async Task TypeSelectedChangedHandle(MediaType type)
        {
            await LoadItems(type);
        }

        private async Task LoadItems(MediaType type)
        {
            try
            {
                await State.StartLoading.Invoke(null);

                if (MediaListApi != null)
                {
                    _ = await MediaListApi.GetList(Items, State, type, StringParameters, List, 1, Cts.Token);

                    if (Items.Count < MinQtdItems) //force reload, if the filters bring few records
                    {
                        _ = await MediaListApi.GetList(Items, State, type, StringParameters, List, 2, Cts.Token);
                    }

                    if (OrderByComments) Items = Items.OrderByDescending(o => int.Parse(o.comments?.Split(",")[^1] ?? "0", System.Globalization.CultureInfo.InvariantCulture)).ToHashSet();

                    await State.FinishLoading.Invoke(Items);
                }
            }
            catch (Exception ex)
            {
                await State.ShowError.Invoke(ex.Message);
            }
        }

        private string GetCustomExpandUrl()
        {
            if (List != null)
            {
                return string.Create(CultureInfo.InvariantCulture, $"/{Culture}/list/{(int)List}");
            }

            if (CustomExpand.NotEmpty())
            {
                return CustomExpand.CustomFormat(TypeSelected.ToString());
            }

            return string.Empty;
        }

        private async Task OpenPopupMedia(MediaDetail media)
        {
            try
            {
                string? tmdbId;

                if (IsImdb)
                {
                    //for now, only tv series (imdb) need this kind of workaround (tmdb api only work with imdb id from movies - this info is not documented)
                    tmdbId = await ExternalIdApi.GetTmdbId(media.MediaType, media.tmdb_id, Cts.Token);
                }
                else
                {
                    tmdbId = media.tmdb_id;
                }

                if (tmdbId.Empty())
                {
                    await ShowError("Unable to display this content. Please try again later.");
                    return;
                }

                if (media.MediaType == MediaType.person)
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
                        await DialogService.MediaPopup(Watching, Wish, media.MediaType, tmdbId, Culture);
                    }
                    else
                    {
                        Navigation.NavigateTo($"/{Culture}/media/{media.MediaType}/{tmdbId}");
                    }
                }
            }
            catch (Exception ex)
            {
                await State.ShowError.Invoke(ex.Message);
            }
        }
    }
}