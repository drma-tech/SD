using Microsoft.AspNetCore.Components;
using MudBlazor;
using SD.Shared.Models.List;
using System.Globalization;

namespace SD.WEB.Modules.Media
{
    public partial class MediaPopup
    {
        [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }
        [Parameter][EditorRequired] public string? Culture { get; set; }

        [Parameter] public WatchingList? Watching { get; set; }
        [Parameter] public WishList? Wish { get; set; }
        [Parameter] public EventCallback<WatchingList?> WatchingChanged { get; set; }
        [Parameter] public EventCallback<WishList?> WishChanged { get; set; }

        [Parameter] public string? TmdbId { get; set; }
        [Parameter] public MediaType? Type { get; set; }

        public RenderControlState<MediaDetail> State { get; set; } = new(obj => obj == null);
        private MediaDetail? Media { get; set; }
        public string? ImdbId { get; set; }
        public string? EnglishTitle { get; set; }

        public RenderControlState<RatingsCache> RatingsState { get; set; } = new(obj => obj?.Data == null);
        private RatingsCache? _ratingsCache;

        protected override void OnInitialized()
        {
            if (string.IsNullOrEmpty(TmdbId)) throw new NotificationException("TmdbId is required");
            if (Type == null) throw new NotificationException("Type is required");

            WatchingListApi.DataChanged += model =>
            {
                Watching = model;
                _ = WatchingChanged.InvokeAsync(model);
                StateHasChanged();
            };
            WishListApi.DataChanged += model =>
            {
                Wish = model;
                _ = WishChanged.InvokeAsync(model);
                StateHasChanged();
            };
        }

        protected override List<string?> GetParameterKey()
        {
            return [
                TmdbId,
                Type.ToString(),
            ];
        }

        protected override async Task LoadParameterDataAsync()
        {
            var lang = (await AppStateStatic.GetContentLanguage(JsRuntime, Cts.Token)).GetFieldSettings(translate: false).Name ?? "en-US";
            Media = await TmdbApi.GetMediaDetail(TmdbId, Type!.Value, lang, State, Cts.Token);
            Media.Videos = Media.Videos.Reverse();

            ImdbId = await ExternalIdApi.GetImdbId(Type, TmdbId, Cts.Token);

            EnglishTitle = Media?.original_title;
            await MudDialog!.SetTitleAsync(Media?.title);

            if (Media != null && !string.Equals(Media.original_language, "en", StringComparison.OrdinalIgnoreCase))
            {
                //title must be in English
                var enMedia = await TmdbApi.GetMediaDetail(TmdbId, Type!.Value, "en-US", state: null, Cts.Token);
                EnglishTitle = enMedia.title;
            }

            EnglishTitle = EnglishTitle?.Replace("&", "", StringComparison.Ordinal);

            StateHasChanged();

            if (Media?.MediaType == MediaType.movie)
            {
                _ratingsCache = await CacheRatingsApi.GetMovieRatings(ImdbId, Media?.tmdb_id, EnglishTitle, Media?.release_date, Media?.rating?.ToString("#.#", CultureInfo.InvariantCulture), RatingsState, Cts.Token);
            }
            else
            {
                _ratingsCache = await CacheRatingsApi.GetShowRatings(ImdbId, Media?.tmdb_id, EnglishTitle, Media?.release_date, Media?.rating?.ToString("#.#", CultureInfo.InvariantCulture), RatingsState, Cts.Token);
            }
        }

        private async Task Add()
        {
            if (Media == null) throw new NotificationException("Media is required");

            try
            {
                Wish ??= new WishList(AppStateStatic.UserId);

                var item = new WishListItem(Media.tmdb_id, Media.title, Media.poster_small?.Replace(TmdbOptions.SmallPosterPath, "", StringComparison.Ordinal), Media.runtime);

                Wish = await WishListApi.Add(Media.MediaType, Wish, item, AppStateStatic.ActiveProduct, Cts.Token);
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task Remove()
        {
            if (Media == null) throw new NotificationException("Media is required");

            try
            {
                Wish = await WishListApi.Remove(Media.MediaType, Media?.tmdb_id, Cts.Token);
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task IsWatched()
        {
            if (Media == null) throw new NotificationException("Media is required");

            try
            {
                var hasCollection = Media.MediaType == MediaType.tv || (Media.Collection.Count != 0 && Media.Collection.Count > 1);

                if (hasCollection)
                {
                    var watching = Watching ?? new WatchingList(AppStateStatic.UserId);
                    var collectionId = Media.MediaType == MediaType.movie ? Media.collectionId?.ToString(CultureInfo.InvariantCulture) : Media.tmdb_id;

                    await DialogService.SelectItemsCollection(
                        Media.Collection,
                        watching.GetWatchingItems(Media.MediaType, collectionId),
                        new EventCallbackFactory().Create(this, async (ISet<string> list) => await SelectedItemsChanged(Media, list, Media.Collection.Count)));
                }
                else
                {
                    await ShowWarning(Translations.Notification.NoFollow);
                }
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task SelectedItemsChanged(MediaDetail media, ISet<string> items, int collectionItemsCount)
        {
            //watching list

            WatchingListItem item;

            if (media.MediaType == MediaType.movie)
            {
                item = new WatchingListItem(media.collectionId?.ToString(CultureInfo.InvariantCulture), media.collectionName, media.collectionLogo?.Replace(TmdbOptions.SmallPosterPath, "", StringComparison.Ordinal), collectionItemsCount, items);
            }
            else
            {
                item = new WatchingListItem(media.tmdb_id, media.title, media.poster_small?.Replace(TmdbOptions.SmallPosterPath, "", StringComparison.Ordinal), collectionItemsCount, items);
            }

            Watching = await WatchingListApi.Add(Type, Watching, item, AppStateStatic.ActiveProduct, Cts.Token);
        }

        private async Task IsNotWatched()
        {
            if (Media == null) throw new NotificationException("Media is required");

            try
            {
                //watching

                var hasCollection = Media.Collection.Count != 0 && Media.Collection.Count > 1;

                if (hasCollection)
                {
                    Watching = Media.MediaType switch
                    {
                        MediaType.movie => await WatchingListApi.Remove(Media.MediaType, Media.collectionId?.ToString(CultureInfo.InvariantCulture), Media.tmdb_id, Cts.Token),
                        MediaType.tv => await WatchingListApi.Remove(Media.MediaType, Media.tmdb_id, tmdbId: null, Cts.Token),
                        _ => Watching,
                    };
                }
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private void VideoClick(Video item)
        {
            Navigation.NavigateTo($"/{Culture}/video/{$"{item.key}|{item.name}".SimpleEncrypt()}");
        }
    }
}