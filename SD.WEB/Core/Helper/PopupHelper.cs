using Microsoft.AspNetCore.Components;
using MudBlazor;
using SD.WEB.Modules.Auth;
using SD.WEB.Modules.Collections.Components;
using SD.WEB.Modules.Platform.Components;
using SD.WEB.Modules.Profile.Components;
using SD.WEB.Modules.Profile.Resources;
using SD.WEB.Modules.Subscription.Components;
using SD.WEB.Shared;

namespace SD.WEB.Core.Helper;

public static class PopupHelper
{
    public static readonly EventCallbackFactory Factory = new();

    public static async Task CollectionPopup(this IDialogService service, WatchedList? watched, WatchingList? watching, WishList? wish, MediaType? type,
        string? collectionId, bool showPrivateAction, bool isAuthenticated)
    {
        var parameters = new DialogParameters<CollectionPopup>
        {
            { x => x.CollectionId, collectionId },
            { x => x.Type, type },
            { x => x.Watched, watched },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? lst) => { watched = lst; }) },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList ? lst) => { watching = lst; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList ? lst) => { wish = lst; }) },
            { x => x.ShowPrivateAction, showPrivateAction },
            { x => x.IsAuthenticated, isAuthenticated },
        };

        await service.ShowAsync<CollectionPopup>(null, parameters, Options(MaxWidth.Medium));
    }

    public static async Task CompleteListPopup(this IDialogService service, string? titleHead, WatchedList? watched, WatchingList? watching, WishList? wish,
        HashSet<MediaDetail> items, bool showPrivateAction, bool isAuthenticated)
    {
        var parameters = new DialogParameters<CompleteListPopup>
        {
            { x => x.TitleHead, titleHead },
            { x => x.Watched, watched },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.Items, items },
            { x => x.ItemsChanged, Factory.Create(new object(), (HashSet<MediaDetail> lst) => { items = lst; }) },
            { x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? lst) => { watched = lst; }) },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList ? lst) => { watching = lst; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList ? lst) => { wish = lst; }) },

            { x => x.ShowPrivateAction, showPrivateAction },
            { x => x.IsAuthenticated, isAuthenticated },
        };

        await service.ShowAsync<CompleteListPopup>(titleHead, parameters, Options(MaxWidth.Large));
    }

    public static async Task MediaPopup(this IDialogService service, WatchedList? watched, WatchingList? watching, WishList? wish, MediaType? type, string? tmdbId,
        bool showPrivateAction, bool isAuthenticated)
    {
        var parameters = new DialogParameters<MediaPopup>
        {
            { x => x.TmdbId, tmdbId },
            { x => x.Type, type },
            { x => x.Watched, watched },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? lst) => { watched = lst; }) },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList ? lst) => { watching = lst; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList ? lst) => { wish = lst; }) },
            { x => x.ShowPrivateAction, showPrivateAction },
            { x => x.IsAuthenticated, isAuthenticated },
        };

        await service.ShowAsync<MediaPopup>(null, parameters, Options(MaxWidth.Medium));
    }

    public static async Task MySuggestionsPopup(this IDialogService service, SD.Shared.Models.MySuggestions? mySuggestions,
        EventCallback<SD.Shared.Models.MySuggestions>? mySuggestionsChanged)
    {
        var parameters = new DialogParameters<MySuggestionsPopup>
        {
            { x => x.MySuggestions, mySuggestions },
            { x => x.MySuggestionsChanged, mySuggestionsChanged },
        };

        await service.ShowAsync<MySuggestionsPopup>("Suggestion Filters", parameters, Options(MaxWidth.Small));
    }

    public static async Task MyWatchingListPopup(this IDialogService service, RenderControlCore<WatchingList?>? core, MediaType type,
        WatchedList? watched, WatchingList? watching, WishList? wish, bool showPrivateAction, bool isAuthenticated, string? userId)
    {
        var parameters = new DialogParameters<MyWatchingListPopup>
        {
            { x => x.Core, core },
            { x => x.MediaType, type },
            { x => x.Watched, watched },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }) },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }) },
            { x => x.ShowPrivateAction, showPrivateAction },
            { x => x.IsAuthenticated, isAuthenticated },
            { x => x.UserId, userId }
        };

        var Title = type == MediaType.movie ? Translations.MyMovieWatching : Translations.MyShowWatching;
        var Quantity = type == MediaType.movie ? watching?.Movies.Count ?? 0 : watching?.Shows.Count ?? 0;

        await service.ShowAsync<MyWatchingListPopup>(Title.Format(Quantity), parameters, Options(MaxWidth.Large));
    }

    public static async Task MyWishListPopup(this IDialogService service, RenderControlCore<WishList?>? core, WatchedList? watched, WatchingList? watching, WishList? wish,
        MediaType type, bool showPrivateAction, bool isAuthenticated, string? userId)
    {
        var parameters = new DialogParameters<MyWishListPopup>
        {
            { x => x.Core, core },
            { x => x.MediaType, type },
            { x => x.Watched, watched },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }) },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }) },
            { x => x.ShowPrivateAction, showPrivateAction },
            { x => x.IsAuthenticated, isAuthenticated },
            { x => x.UserId, userId }
        };

        var Title = type == MediaType.movie ? Translations.MyMovieWishlist : Translations.MyShowWishlist;
        var Quantity = type == MediaType.movie ? wish?.Movies.Count ?? 0 : wish?.Shows.Count ?? 0;

        await service.ShowAsync<MyWishListPopup>(Title.Format(Quantity), parameters, Options(MaxWidth.Large));
    }

    public static async Task OpenAccountPopup(this IDialogService service, bool isAuthenticated)
    {
        var parameters = new DialogParameters<ProfilePopup>
        {
            { x => x.IsAuthenticated, isAuthenticated }
        };

        await service.ShowAsync<ProfilePopup>(Translations.MyProfile, parameters, Options(MaxWidth.ExtraSmall));
    }

    public static async Task PlataformPopup(this IDialogService service, ProviderModel? provider, WatchedList? watched, WatchingList? watching, WishList? wish,
        bool showPrivateAction, string? watchRegion, string? providerId, bool isAuthenticated)
    {
        var parameters = new DialogParameters<PlataformPopup>
        {
            { x => x.Provider, provider },
            { x => x.Watched, watched },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }) },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }) },
            { x => x.ShowPrivateAction, showPrivateAction },
            { x => x.WatchRegion, watchRegion },
            { x => x.ProviderId, providerId },
            { x => x.IsAuthenticated, isAuthenticated }
        };

        await service.ShowAsync<PlataformPopup>(provider?.name, parameters, Options(MaxWidth.Large));
    }

    public static async Task SeasonPopup(this IDialogService service, string? showTitle, string? showSeasonName, string? tmdbId, int? seasonNumber, bool isAuthenticated)
    {
        var parameters = new DialogParameters<SeasonPopup>
        {
            { x => x.TmdbId, tmdbId },
            { x => x.SeasonNumber, seasonNumber },
            { x => x.IsAuthenticated, isAuthenticated }
        };

        await service.ShowAsync<SeasonPopup>($"{showTitle} ({showSeasonName})", parameters, Options(MaxWidth.Medium));
    }

    public static async Task SelectItemsCollection(this IDialogService service, List<Collection> items, HashSet<string> selectedItems,
        EventCallback<HashSet<string>> itemsChanged)
    {
        var parameters = new DialogParameters<SelectItemsCollection>
        {
            { x => x.ItemsCollection, items },
            { x => x.SelectedItems, selectedItems },
            { x => x.SelectedItemsChanged, itemsChanged }
        };

        await service.ShowAsync<SelectItemsCollection>("What items have you already watched?", parameters, Options(MaxWidth.ExtraSmall));
    }

    public static async Task SettingsPopup(this IDialogService service)
    {
        await service.ShowAsync<SettingsPopup>(GlobalTranslations.Settings, Options(MaxWidth.Small));
    }

    public static async Task SubscriptionPopup(this IDialogService service, bool isAuthenticated)
    {
        var parameters = new DialogParameters<SubscriptionPopup>
        {
            { x => x.IsAuthenticated, isAuthenticated }
        };

        await service.ShowAsync<SubscriptionPopup>(Modules.Subscription.Resources.Translations.MySubscription, parameters, Options(MaxWidth.Medium));
    }

    public static DialogOptions Options(MaxWidth width)
    {
        return new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            Position = DialogPosition.Center,
            MaxWidth = width
        };
    }
}