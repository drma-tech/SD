using Microsoft.AspNetCore.Components;

namespace SD.WEB.Modules.Profile.Components
{
    public partial class WatchingComponent
    {
        [Parameter][EditorRequired] public RenderControlState<WatchingList> MovieState { get; set; }
        [Parameter][EditorRequired] public RenderControlState<WatchingList> TvState { get; set; }

        [Parameter][EditorRequired] public bool ShowHeader { get; set; }
        [Parameter][EditorRequired] public bool FullScreen { get; set; }
        [Parameter][EditorRequired] public WatchingList? Watching { get; set; }
        [Parameter][EditorRequired] public WishList? Wish { get; set; }
        [Parameter][EditorRequired] public string? Culture { get; set; }

        [Parameter] public MediaType? TypeParam { get; set; }
        [Parameter] public string? CustomTitle { get; set; }

        private MediaType _type { get; set; } = MediaType.movie;

        private ISet<WatchingListItem> Items(MediaType type) => type == MediaType.movie ? Watching?.Movies ?? new HashSet<WatchingListItem>() : Watching?.Shows ?? new HashSet<WatchingListItem>();

        private int GetTotalItems => FullScreen ? AccountProduct.Premium.GetRestrictions().Watching : 7;

        public bool Updating { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MovieState.CustomMessageWarning = Translations.Module.Profile.FollowTitle;
            TvState.CustomMessageWarning = Translations.Module.Profile.FollowTitle;
        }

        private async Task OpenCompleteList(MediaType type)
        {
            await DialogService.MyWatchingListPopup(type == MediaType.movie ? MovieState : TvState, type, Watching, Wish, Culture);
        }

        public async Task ShowMediaPopup(MediaType type, string? tmdbId, string? name)
        {
            if (tmdbId.NotEmpty()) await DialogService.MediaPopup(Watching, Wish, type, tmdbId, Culture);
        }

        public async Task ShowCollectionPopup(MediaType type, string? collectionId, string? name)
        {
            if (collectionId.NotEmpty()) await DialogService.CollectionPopup(Watching, Wish, type, collectionId, false);
        }

        // private async Task ImportFromWatched(MediaType type)
        // {
        //     try
        //     {
        //         var watching = await WatchingApi.Get(type == MediaType.movie ? ActionsMovie : ActionsTv, Cts.Token); //get a new one (memory may be compromised/corrupted)
        //         var lang = (await AppStateStatic.GetContentLanguage(JsRuntime, Cts.Token)).GetName(false) ?? "en-US";

        //         if (type == MediaType.movie)
        //         {
        //             watching?.Movies.Clear();
        //             Updating = true;
        //             StateHasChanged();
        //             await ActionsMovie.StartProcessing.Invoke(null);

        //             foreach (var tmdbId in Watched?.GetItems(MediaType.movie) ?? [])
        //             {
        //                 try
        //                 {
        //                     var media = await TmdbApi.GetMediaDetail(tmdbId, MediaType.movie, lang, null, Cts.Token);
        //                     var hasCollection = media.Collection.Any() && media.Collection.Count > 1;

        //                     if (hasCollection && watching!.DeletedMovies.All(a => a != media.collectionId.ToString()))
        //                     {
        //                         var item = watching?.GetItem(MediaType.movie, media.collectionId?.ToString());

        //                         if (item == null)
        //                         {
        //                             var items = new HashSet<string> { tmdbId };

        //                             item = new WatchingListItem(media.collectionId?.ToString(), media.collectionName, media.collectionLogo?.Replace(TmdbOptions.SmallPosterPath, ""), media.Collection.Count, items);
        //                         }
        //                         else
        //                         {
        //                             item.maxItems = media.Collection.Count;
        //                             item.watched.Add(tmdbId);
        //                         }

        //                         watching?.AddItem(MediaType.movie, item);
        //                     }
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     if (ex.Message.Contains("The resource you requested could not be found"))
        //                     {
        //                         Watched = await WatchedApi.Remove(MediaType.movie, tmdbId, Cts.Token);
        //                     }
        //                     else
        //                     {
        //                         throw;
        //                     }
        //                 }
        //             }

        //             watching = await WatchingApi.Sync(MediaType.movie, watching, Cts.Token);

        //             Updating = false;
        //             StateHasChanged();
        //             await ActionsMovie.FinishProcessing.Invoke(watching);
        //         }
        //         else
        //         {
        //             watching?.Shows.Clear();
        //             Updating = true;
        //             StateHasChanged();
        //             await ActionsTv.StartProcessing.Invoke(null);

        //             foreach (var tmdbId in Watched?.GetItems(MediaType.tv) ?? [])
        //             {
        //                 try
        //                 {
        //                     var media = await TmdbApi.GetMediaDetail(tmdbId, MediaType.tv, lang, null, Cts.Token);
        //                     var hasCollection = media.Collection.Any() && media.Collection.Count > 1;

        //                     if (hasCollection && watching!.DeletedShows.All(a => a != media.tmdb_id))
        //                     {
        //                         var item = watching?.GetItem(MediaType.tv, media.tmdb_id);

        //                         if (item == null)
        //                         {
        //                             var items = new HashSet<string> { media.Collection.OrderBy(o => o.release_date ?? DateTime.MaxValue).FirstOrDefault()?.id ?? "" };

        //                             item = new WatchingListItem(media.tmdb_id, media.title, media.poster_small?.Replace(TmdbOptions.SmallPosterPath, ""), media.Collection.Count, items);
        //                         }
        //                         else
        //                         {
        //                             item.maxItems = media.Collection.Count;
        //                             item.watched.Add(media.Collection.OrderBy(o => o.release_date ?? DateTime.MaxValue).FirstOrDefault()?.id ?? "");
        //                         }

        //                         watching?.AddItem(MediaType.tv, item);
        //                     }
        //                 }
        //                 catch (Exception ex)
        //                 {
        //                     if (ex.Message.Contains("The resource you requested could not be found"))
        //                     {
        //                         Watched = await WatchedApi.Remove(MediaType.tv, tmdbId, Cts.Token);
        //                     }
        //                     else
        //                     {
        //                         throw;
        //                     }
        //                 }
        //             }

        //             watching = await WatchingApi.Sync(MediaType.tv, watching, Cts.Token);

        //             Updating = false;
        //             StateHasChanged();
        //             await ActionsTv.FinishProcessing.Invoke(watching);
        //         }

        //         StateHasChanged();
        //     }
        //     catch (Exception ex)
        //     {
        //         Updating = false;
        //         await ActionsMovie.ShowError.Invoke(ex.Message);
        //         await ActionsTv.ShowError.Invoke(ex.Message);
        //         await ProcessException(ex);
        //     }
        // }
    }
}