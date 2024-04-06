using Blazorise;
using Microsoft.AspNetCore.Components;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Support;
using SD.WEB.Modules.News.Components;
using SD.WEB.Modules.Profile;
using SD.WEB.Modules.Provider.Components;
using SD.WEB.Modules.Subscription.Components;
using SD.WEB.Modules.Suggestions.Components;
using SD.WEB.Modules.Suggestions.Core;
using SD.WEB.Modules.Suggestions.Interface;
using SD.WEB.Modules.Support.Component;
using SD.WEB.Modules.Trailers.Components;
using SD.WEB.Shared;

namespace SD.WEB.Core
{
    public static class PopupHelper
    {
        public static readonly EventCallbackFactory Factory = new();

        public static async Task CollectionPopup(this IModalService service, WatchedList? watched, WatchingList? watching, WishList? wish, MediaType? type,
            string? collection_id, bool showPrivateAction, bool IsAuthenticated)
        {
            await service.Show<CollectionPopup>(null, x =>
            {
                x.Add(x => x.collection_id, collection_id);
                x.Add(x => x.type, type);
                x.Add(x => x.Watched, watched);
                x.Add(x => x.Watching, watching);
                x.Add(x => x.Wish, wish);
                x.Add(x => x.WatchedChanged, Factory.Create(new(), (WatchedList? list) => { watched = list; }));
                x.Add(x => x.WatchingChanged, Factory.Create(new(), (WatchingList? list) => { watching = list; }));
                x.Add(x => x.WishChanged, Factory.Create(new(), (WishList? list) => { wish = list; }));
                x.Add(x => x.ShowPrivateAction, showPrivateAction);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
            }, Options(ModalSize.Large));
        }

        public static async Task CompleteListPopup(this IModalService service, string? TitleHead, WatchedList? watched, WatchingList? watching, WishList? wish,
            HashSet<MediaDetail> Items, bool showPrivateAction, bool IsAuthenticated)
        {
            await service.Show<CompleteListPopup>(null, x =>
            {
                x.Add(x => x.TitleHead, TitleHead);
                x.Add(x => x.Watched, watched);
                x.Add(x => x.Watching, watching);
                x.Add(x => x.Wish, wish);
                x.Add(x => x.WatchedChanged, Factory.Create(new(), (WatchedList? list) => { watched = list; }));
                x.Add(x => x.WatchingChanged, Factory.Create(new(), (WatchingList? list) => { watching = list; }));
                x.Add(x => x.WishChanged, Factory.Create(new(), (WishList? list) => { wish = list; }));
                x.Add(x => x.Items, Items);
                x.Add(x => x.ShowPrivateAction, showPrivateAction);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
            }, Options(ModalSize.ExtraLarge));
        }

        public static async Task CompleteListPopup(this IModalService service, string? TitleHead, WatchedList? watched, WatchingList? watching, WishList? wish, HashSet<MediaDetail> Items,
            IMediaListApi? MediaListApi, EnumLists? List, int MaxItens, bool IsIMDB, MediaType? TypeSelected, Dictionary<string, string> StringParameters,
            bool showPrivateAction, bool IsAuthenticated)
        {
            await service.Show<CompleteListPopup>(null, x =>
            {
                x.Add(x => x.TitleHead, TitleHead);
                x.Add(x => x.Watched, watched);
                x.Add(x => x.Watching, watching);
                x.Add(x => x.Wish, wish);
                x.Add(x => x.Items, Items);
                x.Add(x => x.WatchedChanged, Factory.Create(new(), (WatchedList? list) => { watched = list; }));
                x.Add(x => x.WatchingChanged, Factory.Create(new(), (WatchingList? list) => { watching = list; }));
                x.Add(x => x.WishChanged, Factory.Create(new(), (WishList? list) => { wish = list; }));

                x.Add(x => x.MediaListApi, MediaListApi);
                x.Add(x => x.List, List);
                x.Add(x => x.MaxItens, MaxItens);
                x.Add(x => x.IsIMDB, IsIMDB);
                x.Add(x => x.TypeSelected, TypeSelected);
                x.Add(x => x.StringParameters, StringParameters);
                x.Add(x => x.ShowPrivateAction, showPrivateAction);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
            }, Options(ModalSize.ExtraLarge));
        }

        public static async Task CompleteListProvider(this IModalService service, string? cardHeader, AllProviders? allProviders, WatchedList? watched,
            WatchingList? watching, WishList? wish, bool showPrivateAction, bool IsAuthenticated)
        {
            await service.Show<CompleteListProvider>(null, x =>
            {
                x.Add(x => x.CardHeader, cardHeader);
                x.Add(x => x.AllProviders, allProviders);
                x.Add(x => x.Watched, watched);
                x.Add(x => x.Watching, watching);
                x.Add(x => x.Wish, wish);
                x.Add(x => x.ShowPrivateAction, showPrivateAction);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
            }, Options(ModalSize.ExtraLarge));
        }

        public static async Task MediaPopup(this IModalService service, WatchedList? watched, WatchingList? watching, WishList? wish, MediaType? type, string? tmdb_id,
            bool showPrivateAction, bool IsAuthenticated)
        {
            await service.Show<MediaPopup>(null, x =>
            {
                x.Add(x => x.tmdb_id, tmdb_id);
                x.Add(x => x.type, type);
                x.Add(x => x.Watched, watched);
                x.Add(x => x.Watching, watching);
                x.Add(x => x.Wish, wish);
                x.Add(x => x.WatchedChanged, Factory.Create(new(), (WatchedList? list) => { watched = list; }));
                x.Add(x => x.WatchingChanged, Factory.Create(new(), (WatchingList? list) => { watching = list; }));
                x.Add(x => x.WishChanged, Factory.Create(new(), (WishList? list) => { wish = list; }));
                x.Add(x => x.ShowPrivateAction, showPrivateAction);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
            }, Options(ModalSize.Large));
        }

        public static async Task MySuggestionsPopup(this IModalService service, SD.Shared.Models.MySuggestions? MySuggestions,
            EventCallback<SD.Shared.Models.MySuggestions>? MySuggestionsChanged)
        {
            await service.Show<MySuggestionsPopup>(null, x =>
            {
                x.Add(x => x.MySuggestions, MySuggestions);
                x.Add(x => x.MySuggestionsChanged, MySuggestionsChanged);
            }, Options(ModalSize.Default));
        }

        public static async Task MyWatchingListPopup(this IModalService service, RenderControlCore<WatchingList?>? Core, MediaType MediaType,
            WatchedList? watched, WatchingList? watching, WishList? wish, bool showPrivateAction, bool IsAuthenticated, string? UserId)
        {
            await service.Show<MyWatchingListPopup>(null, x =>
            {
                x.Add(x => x.Core, Core);
                x.Add(x => x.MediaType, MediaType);
                x.Add(x => x.Watched, watched);
                x.Add(x => x.Watching, watching);
                x.Add(x => x.Wish, wish);
                x.Add(x => x.WatchedChanged, Factory.Create(new(), (WatchedList? list) => { watched = list; }));
                x.Add(x => x.WatchingChanged, Factory.Create(new(), (WatchingList? list) => { watching = list; }));
                x.Add(x => x.WishChanged, Factory.Create(new(), (WishList? list) => { wish = list; }));
                x.Add(x => x.ShowPrivateAction, showPrivateAction);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
                x.Add(x => x.UserId, UserId);
            }, Options(ModalSize.ExtraLarge));
        }

        public static async Task MyWishListPopup(this IModalService service, RenderControlCore<WishList?>? Core,
            WatchedList? watched, WatchingList? watching, WishList? wish, MediaType type, bool showPrivateAction, bool IsAuthenticated, string? UserId)
        {
            await service.Show<MyWishListPopup>(null, x =>
            {
                x.Add(x => x.Core, Core);
                x.Add(x => x.MediaType, type);
                x.Add(x => x.Watched, watched);
                x.Add(x => x.Watching, watching);
                x.Add(x => x.Wish, wish);
                x.Add(x => x.WatchedChanged, Factory.Create(new(), (WatchedList? list) => { watched = list; }));
                x.Add(x => x.WatchingChanged, Factory.Create(new(), (WatchingList? list) => { watching = list; }));
                x.Add(x => x.WishChanged, Factory.Create(new(), (WishList? list) => { wish = list; }));
                x.Add(x => x.ShowPrivateAction, showPrivateAction);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
                x.Add(x => x.UserId, UserId);
            }, Options(ModalSize.ExtraLarge));
        }

        public static async Task NewsPopup(this IModalService service)
        {
            await service.Show<NewsPopup>(null, x => { }, Options(ModalSize.ExtraLarge));
        }

        public static async Task NewTicket(this IModalService service, TicketType TicketType, EventCallback<TicketModel> Inserted, bool IsAuthenticated, string? UserId)
        {
            await service.Show<NewTicket>(null, x =>
            {
                x.Add(x => x.TicketType, TicketType);
                x.Add(x => x.Inserted, Inserted);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
                x.Add(x => x.UserId, UserId);
            }, Options(ModalSize.Default));
        }

        public static async Task ProviderPopup(this IModalService service, ProviderModel? provider, WatchedList? watched, WatchingList? watching, WishList? wish,
            bool showPrivateAction, string? WatchRegion, string? ProviderId, bool IsAuthenticated)
        {
            await service.Show<ProviderPopup>(null, x =>
            {
                x.Add(x => x.provider, provider);
                x.Add(x => x.Watched, watched);
                x.Add(x => x.Watching, watching);
                x.Add(x => x.Wish, wish);
                x.Add(x => x.WatchedChanged, Factory.Create(new(), (WatchedList? list) => { watched = list; }));
                x.Add(x => x.WatchingChanged, Factory.Create(new(), (WatchingList? list) => { watching = list; }));
                x.Add(x => x.WishChanged, Factory.Create(new(), (WishList? list) => { wish = list; }));
                x.Add(x => x.ShowPrivateAction, showPrivateAction);
                x.Add(x => x.WatchRegion, WatchRegion);
                x.Add(x => x.ProviderId, ProviderId);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
            }, Options(ModalSize.ExtraLarge));
        }

        public static async Task SearchPopup(this IModalService service, string? titleHead, string? search, WatchedList? watched, WatchingList? watching, WishList? wish,
            bool showPrivateAction, bool IsAuthenticated)
        {
            await service.Show<SearchPopup>(null, x =>
            {
                x.Add(x => x.TitleHead, titleHead);
                x.Add(x => x.Search, search);
                x.Add(x => x.Watched, watched);
                x.Add(x => x.Watching, watching);
                x.Add(x => x.Wish, wish);
                x.Add(x => x.WatchedChanged, Factory.Create(new(), (WatchedList? list) => { watched = list; }));
                x.Add(x => x.WatchingChanged, Factory.Create(new(), (WatchingList? list) => { watching = list; }));
                x.Add(x => x.WishChanged, Factory.Create(new(), (WishList? list) => { wish = list; }));
                x.Add(x => x.ShowPrivateAction, showPrivateAction);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
            }, Options(ModalSize.ExtraLarge));
        }

        public static async Task SeasonPopup(this IModalService service, string? ShowTitle, string? ShowSeasonName, string? tmdb_id, int? season_number)
        {
            await service.Show<SeasonPopup>(null, x =>
            {
                x.Add(x => x.ShowTitle, ShowTitle);
                x.Add(x => x.ShowSeasonName, ShowSeasonName);
                x.Add(x => x.tmdb_id, tmdb_id);
                x.Add(x => x.season_number, season_number);
            }, Options(ModalSize.ExtraLarge));
        }

        public static async Task SelectItemsCollection(this IModalService service, List<Collection> Items, HashSet<string> SelectedItems, EventCallback<HashSet<string>> ItemsChanged)
        {
            await service.Show<SelectItemsCollection>(null, x =>
            {
                x.Add(x => x.ItemsCollection, Items);
                x.Add(x => x.SelectedItems, SelectedItems);
                x.Add(x => x.SelectedItemsChanged, ItemsChanged);
            }, Options(ModalSize.Default));
        }

        public static async Task SubscriptionPopup(this IModalService service, ClientePrincipal? client, bool IsAuthenticated)
        {
            await service.Show<SubscriptionPopup>(null, x =>
            {
                x.Add(x => x.Client, client);
                x.Add(x => x.IsAuthenticated, IsAuthenticated);
            }, Options(ModalSize.Large));
        }

        public static async Task TrailersPopup(this IModalService service)
        {
            await service.Show<TrailersPopup>(null, x => { }, Options(ModalSize.ExtraLarge));
        }

        public static async Task OpenPopup<TComponent>(this IModalService service, Action<ModalProviderParameterBuilder<TComponent>> parameters, ModalSize size)
            where TComponent : IComponent
        {
            await service.Show(null, parameters, Options(size));
        }

        private static ModalInstanceOptions Options(ModalSize size) => new()
        {
            UseModalStructure = false,
            Centered = true,
            Size = size
        };
    }
}