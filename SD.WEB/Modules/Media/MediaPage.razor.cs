using Microsoft.AspNetCore.Components;
using SD.Shared.Models.List;
using System.Globalization;

namespace SD.WEB.Modules.Media
{
    public partial class MediaPage
    {
        public WatchingList? Watching { get; set; }
        public WishList? Wish { get; set; }
        public Country? Region { get; set; }

        [Parameter] public string? id { get; set; }
        [Parameter] public string? type { get; set; }

        public string? TmdbId => id;

        public MediaType? Type => type switch
        {
            "movie" => MediaType.movie,
            "tv" => MediaType.tv,
            _ => null,
        };

        private MediaDetail? Media { get; set; }
        public string? ImdbId { get; set; }
        public string? EnglishTitle { get; set; }

        private RenderControlState<MediaDetail> Actions { get; set; } = new(obj => obj == null);
        private RenderControlState<RatingsCache> RatingsActions { get; set; } = new(obj => obj?.Data == null);
        private RatingsCache? _ratingsCache;

        private RenderControlState<ICollection<MediaDetail>> RecommendationsActions { get; set; } = new(lst => lst == null || lst.Empty());
        public IEnumerable<MediaDetail> Recommendations { get; set; } = [];

        protected override List<string?> GetParameterKey()
        {
            return [
                Culture,
                TmdbId,
                Type.ToString(),
            ];
        }

        protected override async Task LoadParameterDataAsync()
        {
            try
            {
                await Actions.StartLoading.Invoke(null);

                var lang = (await AppStateStatic.GetContentLanguage(JsRuntime, Cts.Token)).GetFieldSettings(translate: false).Name ?? "en-US";
                Media = await TmdbApi.GetMediaDetail(TmdbId, Type!.Value, lang, actions: null, Cts.Token);
                Media.Videos = Media.Videos.Reverse();

                await Actions.FinishLoading.Invoke(Media);

                EnglishTitle = Media?.original_title;

                if (Media != null && !string.Equals(Media.original_language, "en", StringComparison.OrdinalIgnoreCase))
                {
                    //title must be in English
                    var enMedia = await TmdbApi.GetMediaDetail(TmdbId, Type!.Value, "en-US", actions: null, Cts.Token);
                    EnglishTitle = enMedia.title;
                }

                EnglishTitle = EnglishTitle?.Replace("&", "", StringComparison.Ordinal);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                await Actions.ShowError.Invoke(ex.Message);
            }

            try
            {
                ImdbId = await ExternalIdApi.GetImdbId(Type, TmdbId, Cts.Token);

                if (Media?.MediaType == MediaType.movie)
                {
                    _ratingsCache = await CacheRatingsApi.GetMovieRatings(ImdbId, Media?.tmdb_id, EnglishTitle, Media?.release_date, Media?.rating.ToString("#.#", System.Globalization.CultureInfo.InvariantCulture), RatingsActions, Cts.Token);
                }
                else
                {
                    _ratingsCache = await CacheRatingsApi.GetShowRatings(ImdbId, Media?.tmdb_id, EnglishTitle, Media?.release_date, Media?.rating.ToString("#.#", System.Globalization.CultureInfo.InvariantCulture), RatingsActions, Cts.Token);
                }
            }
            catch (Exception ex)
            {
                await RatingsActions.ShowError.Invoke(ex.Message);
            }

            try
            {
                Recommendations = await TmdbRecommendationsApi.GetList(Type, TmdbId, RecommendationsActions, Cts.Token);
            }
            catch (Exception ex)
            {
                await RecommendationsActions.ShowError.Invoke(ex.Message);
            }
        }

        protected override async Task<bool> LoadInteropDataAsync(Microsoft.JSInterop.IJSRuntime JsRuntime)
        {
            Region = await AppStateStatic.GetRegion(IpInfoApi, JsRuntime, Cts.Token);

            return true;
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            Watching = await WatchingApi.Get(actions: null, token);
            Wish = await WishApi.Get(actions: null, token);
        }

        private void OpenPopupMedia(MediaType? type, string? tmdb_id)
        {
            Navigation.NavigateTo($"/{Culture}/media/{type}/{tmdb_id}");
        }

        private async Task Add()
        {
            if (Media == null) throw new NotificationException("Media is required");

            try
            {
                if (!AppStateStatic.IsAuthenticated)
                {
                    await ShowWarning(Translations.Notification.YouMustLogged);
                    return;
                }

                Wish ??= new WishList(AppStateStatic.UserId);

                var item = new WishListItem(Media.tmdb_id, Media.title, Media.poster_small?.Replace(TmdbOptions.SmallPosterPath, "", StringComparison.OrdinalIgnoreCase), Media.runtime);

                Wish = await WishApi.Add(Media.MediaType, Wish, item, AppStateStatic.ActiveProduct, Cts.Token);
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
                Wish = await WishApi.Remove(Media.MediaType, Media?.tmdb_id, Cts.Token);
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
                if (!AppStateStatic.IsAuthenticated)
                {
                    await ShowWarning(Translations.Notification.YouMustLogged);
                    return;
                }

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
                        MediaType.movie => await WatchingApi.Remove(Media.MediaType, Media.collectionId?.ToString(CultureInfo.InvariantCulture), Media.tmdb_id, Cts.Token),
                        MediaType.tv => await WatchingApi.Remove(Media.MediaType, Media.tmdb_id, tmdbId: null, Cts.Token),
                        _ => Watching,
                    };
                }
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task SelectedItemsChanged(MediaDetail media, ISet<string> items, int collectionItemsCount)
        {
            try
            {
                //watching list

                WatchingListItem item;

                if (media.MediaType == MediaType.movie)
                {
                    item = new WatchingListItem(media.collectionId?.ToString(System.Globalization.CultureInfo.InvariantCulture), media.collectionName, media.collectionLogo?.Replace(TmdbOptions.SmallPosterPath, "", StringComparison.OrdinalIgnoreCase), collectionItemsCount, items);
                }
                else
                {
                    item = new WatchingListItem(media.tmdb_id, media.title, media.poster_small?.Replace(TmdbOptions.SmallPosterPath, "", StringComparison.OrdinalIgnoreCase), collectionItemsCount, items);
                }

                Watching = await WatchingApi.Add(Type, Watching, item, AppStateStatic.ActiveProduct, Cts.Token);
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