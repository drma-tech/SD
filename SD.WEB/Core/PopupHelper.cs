using Blazorise;
using Microsoft.AspNetCore.Components;
using SD.WEB.Modules.Profile;
using SD.WEB.Modules.Provider.Components;
using SD.WEB.Modules.Subscription.Components;
using SD.WEB.Modules.Suggestions.Components;
using SD.WEB.Modules.Suggestions.Core;
using SD.WEB.Modules.Suggestions.Interface;
using SD.WEB.Shared;
using MySuggestions = SD.Shared.Models.MySuggestions;

namespace SD.WEB.Core;

public static class PopupHelper
{
    public static readonly EventCallbackFactory Factory = new();

    public static async Task CollectionPopup(this IModalService service, WatchedList? watched, WatchingList? watching,
        WishList? wish, MediaType? type,
        string? collectionId, bool showPrivateAction, bool isAuthenticated)
    {
        await service.Show<CollectionPopup>(null, x =>
        {
            x.Add(x => x.CollectionId, collectionId);
            x.Add(x => x.Type, type);
            x.Add(x => x.Watched, watched);
            x.Add(x => x.Watching, watching);
            x.Add(x => x.Wish, wish);
            x.Add(x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }));
            x.Add(x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }));
            x.Add(x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }));
            x.Add(x => x.ShowPrivateAction, showPrivateAction);
            x.Add(x => x.IsAuthenticated, isAuthenticated);
        }, Options(ModalSize.Large));
    }

    public static async Task CompleteListPopup(this IModalService service, string? titleHead, WatchedList? watched,
        WatchingList? watching, WishList? wish,
        HashSet<MediaDetail> items, bool showPrivateAction, bool isAuthenticated)
    {
        await service.Show<CompleteListPopup>(null, x =>
        {
            x.Add(x => x.TitleHead, titleHead);
            x.Add(x => x.Watched, watched);
            x.Add(x => x.Watching, watching);
            x.Add(x => x.Wish, wish);
            x.Add(x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }));
            x.Add(x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }));
            x.Add(x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }));
            x.Add(x => x.Items, items);
            x.Add(x => x.ShowPrivateAction, showPrivateAction);
            x.Add(x => x.IsAuthenticated, isAuthenticated);
        }, Options(ModalSize.ExtraLarge));
    }

    public static async Task CompleteListPopup(this IModalService service, string? titleHead, WatchedList? watched,
        WatchingList? watching, WishList? wish, HashSet<MediaDetail> items,
        IMediaListApi? mediaListApi, EnumLists? list, bool isImdb, MediaType? typeSelected,
        Dictionary<string, string> stringParameters,
        bool showPrivateAction, bool isAuthenticated, bool commentImage = false)
    {
        await service.Show<CompleteListPopup>(null, x =>
        {
            x.Add(x => x.TitleHead, titleHead);
            x.Add(x => x.Watched, watched);
            x.Add(x => x.Watching, watching);
            x.Add(x => x.Wish, wish);
            x.Add(x => x.Items, items);
            x.Add(x => x.ItemsChanged, Factory.Create(new object(), (HashSet<MediaDetail> list) => { items = list; }));
            x.Add(x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }));
            x.Add(x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }));
            x.Add(x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }));

            x.Add(x => x.MediaListApi, mediaListApi);
            x.Add(x => x.List, list);
            x.Add(x => x.IsImdb, isImdb);
            x.Add(x => x.TypeSelected, typeSelected);
            x.Add(x => x.StringParameters, stringParameters);
            x.Add(x => x.ShowPrivateAction, showPrivateAction);
            x.Add(x => x.IsAuthenticated, isAuthenticated);
            x.Add(x => x.CommentImage, commentImage);
        }, Options(ModalSize.ExtraLarge));
    }

    public static async Task MediaPopup(this IModalService service, WatchedList? watched, WatchingList? watching,
        WishList? wish, MediaType? type, string? tmdbId,
        bool showPrivateAction, bool isAuthenticated)
    {
        await service.Show<MediaPopup>(null, x =>
        {
            x.Add(x => x.TmdbId, tmdbId);
            x.Add(x => x.Type, type);
            x.Add(x => x.Watched, watched);
            x.Add(x => x.Watching, watching);
            x.Add(x => x.Wish, wish);
            x.Add(x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }));
            x.Add(x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }));
            x.Add(x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }));
            x.Add(x => x.ShowPrivateAction, showPrivateAction);
            x.Add(x => x.IsAuthenticated, isAuthenticated);
        }, Options(ModalSize.Large));
    }

    public static async Task MySuggestionsPopup(this IModalService service, MySuggestions? mySuggestions,
        EventCallback<MySuggestions>? mySuggestionsChanged)
    {
        await service.Show<MySuggestionsPopup>(null, x =>
        {
            x.Add(x => x.MySuggestions, mySuggestions);
            x.Add(x => x.MySuggestionsChanged, mySuggestionsChanged);
        }, Options(ModalSize.Default));
    }

    public static async Task MyWatchingListPopup(this IModalService service, RenderControlCore<WatchingList?>? core,
        MediaType mediaType,
        WatchedList? watched, WatchingList? watching, WishList? wish, bool showPrivateAction, bool isAuthenticated,
        string? userId)
    {
        await service.Show<MyWatchingListPopup>(null, x =>
        {
            x.Add(x => x.Core, core);
            x.Add(x => x.MediaType, mediaType);
            x.Add(x => x.Watched, watched);
            x.Add(x => x.Watching, watching);
            x.Add(x => x.Wish, wish);
            x.Add(x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }));
            x.Add(x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }));
            x.Add(x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }));
            x.Add(x => x.ShowPrivateAction, showPrivateAction);
            x.Add(x => x.IsAuthenticated, isAuthenticated);
            x.Add(x => x.UserId, userId);
        }, Options(ModalSize.ExtraLarge));
    }

    public static async Task MyWishListPopup(this IModalService service, RenderControlCore<WishList?>? core,
        WatchedList? watched, WatchingList? watching, WishList? wish, MediaType type, bool showPrivateAction,
        bool isAuthenticated, string? userId)
    {
        await service.Show<MyWishListPopup>(null, x =>
        {
            x.Add(x => x.Core, core);
            x.Add(x => x.MediaType, type);
            x.Add(x => x.Watched, watched);
            x.Add(x => x.Watching, watching);
            x.Add(x => x.Wish, wish);
            x.Add(x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }));
            x.Add(x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }));
            x.Add(x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }));
            x.Add(x => x.ShowPrivateAction, showPrivateAction);
            x.Add(x => x.IsAuthenticated, isAuthenticated);
            x.Add(x => x.UserId, userId);
        }, Options(ModalSize.ExtraLarge));
    }

    public static async Task OpenPopup<TComponent>(this IModalService service,
        Action<ModalProviderParameterBuilder<TComponent>> parameters, ModalSize size)
        where TComponent : IComponent
    {
        await service.Show(null, parameters, Options(size));
    }

    public static async Task ProviderPopup(this IModalService service, ProviderModel? provider, WatchedList? watched,
        WatchingList? watching, WishList? wish,
        bool showPrivateAction, string? watchRegion, string? providerId, bool isAuthenticated)
    {
        await service.Show<ProviderPopup>(null, x =>
        {
            x.Add(x => x.Provider, provider);
            x.Add(x => x.Watched, watched);
            x.Add(x => x.Watching, watching);
            x.Add(x => x.Wish, wish);
            x.Add(x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }));
            x.Add(x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }));
            x.Add(x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }));
            x.Add(x => x.ShowPrivateAction, showPrivateAction);
            x.Add(x => x.WatchRegion, watchRegion);
            x.Add(x => x.ProviderId, providerId);
            x.Add(x => x.IsAuthenticated, isAuthenticated);
        }, Options(ModalSize.ExtraLarge));
    }

    public static async Task SeasonPopup(this IModalService service, string? showTitle, string? showSeasonName,
        string? tmdbId, int? seasonNumber, bool isAuthenticated)
    {
        await service.Show<SeasonPopup>(null, x =>
        {
            x.Add(x => x.ShowTitle, showTitle);
            x.Add(x => x.ShowSeasonName, showSeasonName);
            x.Add(x => x.TmdbId, tmdbId);
            x.Add(x => x.SeasonNumber, seasonNumber);
            x.Add(x => x.IsAuthenticated, isAuthenticated);
        }, Options(ModalSize.ExtraLarge));
    }

    public static async Task SelectItemsCollection(this IModalService service, List<Collection> items,
        HashSet<string> selectedItems, EventCallback<HashSet<string>> itemsChanged)
    {
        await service.Show<SelectItemsCollection>(null, x =>
        {
            x.Add(x => x.ItemsCollection, items);
            x.Add(x => x.SelectedItems, selectedItems);
            x.Add(x => x.SelectedItemsChanged, itemsChanged);
        }, Options(ModalSize.Default));
    }

    public static async Task SettingsPopup(this IModalService service)
    {
        await service.Show<SettingsPopup>(null, x => { }, Options(ModalSize.Default));
    }

    public static async Task SubscriptionPopup(this IModalService service, bool isAuthenticated)
    {
        await service.Show<SubscriptionPopup>(null, x => { x.Add(x => x.IsAuthenticated, isAuthenticated); },
            Options(ModalSize.Large));
    }

    private static ModalInstanceOptions Options(ModalSize size)
    {
        return new ModalInstanceOptions
        {
            UseModalStructure = false,
            Centered = true,
            Size = size
        };
    }
}