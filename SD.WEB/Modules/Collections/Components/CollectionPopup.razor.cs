using Microsoft.AspNetCore.Components;
using MudBlazor;
using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Collections.Core;
using System.Globalization;

namespace SD.WEB.Modules.Collections.Components
{
    public partial class CollectionPopup
    {
        [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

        [Parameter][EditorRequired] public WatchingList? Watching { get; set; }
        [Parameter][EditorRequired] public WishList? Wish { get; set; }
        [Parameter] public EventCallback<WatchingList?> WatchingChanged { get; set; }
        [Parameter] public EventCallback<WishList?> WishChanged { get; set; }

        [Parameter] public string? CollectionId { get; set; }
        [Parameter] public MediaType? Type { get; set; }

        private RenderControlState<TmdbCollection> State { get; } = new(obj => obj == null || obj.parts.Empty());
        private TmdbCollection? Collection { get; set; }

        protected override void OnInitialized()
        {
            if (string.IsNullOrEmpty(CollectionId)) throw new NotificationException("CollectionId is required");
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

        protected override async Task LoadStaticDataAsync()
        {
            await State.StartLoading.Invoke(null);
            var lang = (await AppStateStatic.GetContentLanguage(JsRuntime, Cts.Token)).GetFieldSettings(translate: false).Name ?? "en-US";

            if (Type == MediaType.movie)
            {
                var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "api_key", TmdbOptions.ApiKey },
                    { "language", lang },
                };

                Collection = await TmdbApi.GetCollection(CollectionId, parameters, Cts.Token);
            }
            else
            {
                var show = await TmdbApi.GetMediaDetail(CollectionId, MediaType.tv, lang, actions: null, Cts.Token);

                Collection = new TmdbCollection
                {
                    id = int.Parse(show.tmdb_id ?? "0", CultureInfo.InvariantCulture),
                    name = show.title ?? "error",
                };

                foreach (var season in show.Collection)
                {
                    Collection.parts.Add(new Part
                    {
                        id = int.Parse(season.id ?? "0", CultureInfo.InvariantCulture),
                        season_number = season.SeasonNumber,
                        title = season.title ?? "error",
                        release_date = season.release_date?.ToString(CultureInfo.InvariantCulture) ?? "",
                        poster_path = string.IsNullOrEmpty(season.poster_small) ? "" : TmdbOptions.SmallPosterPath + season.poster_small,
                    });
                }
            }

            await MudDialog!.SetTitleAsync(Collection?.name);
            await State.FinishLoading.Invoke(Collection);
        }

        public void HideModal()
        {
            MudDialog?.Close();
        }

        private async Task IsWatched()
        {
            await DialogService.SelectItemsCollection(
                Collection?.parts.Select(p => p.ConvertToCollection()).ToList() ?? [],
                Watching?.GetWatchingItems(Type, Collection?.id.ToString(CultureInfo.InvariantCulture)).ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [],
                new EventCallbackFactory().Create(this, async (ISet<string> list) => await SelectedItemsChanged(Type, Collection, CollectionId, list, Collection?.parts.Count ?? 0)));
        }

        private async Task SelectedItemsChanged(MediaType? type, TmdbCollection? collection, string? tmdbId, ISet<string> items, int collectionItemsCount)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            ArgumentNullException.ThrowIfNull(collection);
            if (tmdbId.Empty()) throw new ArgumentNullException(nameof(tmdbId));
            if (items.Empty()) throw new ArgumentNullException(nameof(items));

            //watching list

            WatchingListItem item;

            if (type == MediaType.movie)
            {
                item = new WatchingListItem(collection.id.ToString(CultureInfo.InvariantCulture), collection?.name, collection?.poster_path?.Replace(TmdbOptions.SmallPosterPath, "", StringComparison.Ordinal), collectionItemsCount, items);
            }
            else
            {
                var lang = (await AppStateStatic.GetContentLanguage(JsRuntime, Cts.Token)).GetFieldSettings(translate: false).Name ?? "en-US";
                var media = await TmdbApi.GetMediaDetail(tmdbId, type.Value, lang, actions: null, Cts.Token);

                item = new WatchingListItem(tmdbId, media.title, media.poster_small?.Replace(TmdbOptions.SmallPosterPath, "", StringComparison.Ordinal), collectionItemsCount, items);
            }

            Watching = await WatchingListApi.Add(type, Watching, item, AppStateStatic.ActiveProduct, Cts.Token);

            // HideModal(); //error on mudblazor
        }

        private async Task RemoveCollection()
        {
            Watching = await WatchingListApi.Remove(Type, CollectionId, cancellationToken: Cts.Token);

            HideModal();
        }
    }
}
