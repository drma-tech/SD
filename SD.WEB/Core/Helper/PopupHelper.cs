using Microsoft.AspNetCore.Components;
using MudBlazor;
using SD.WEB.Modules.Auth;
using SD.WEB.Modules.Collections.Components;
using SD.WEB.Modules.Help;
using SD.WEB.Modules.Media;
using SD.WEB.Modules.Platform.Components;
using SD.WEB.Modules.Profile.Components;
using SD.WEB.Modules.Subscription.Components;
using SD.WEB.Shared;

namespace SD.WEB.Core.Helper;

public static class PopupHelper
{
    public static readonly EventCallbackFactory Factory = new();

    public static async Task CollectionPopup(this IDialogService service, WatchingList? watching, WishList? wish, MediaType? type, string? collectionId)
    {
        var parameters = new DialogParameters<CollectionPopup>
        {
            { x => x.CollectionId, collectionId },
            { x => x.Type, type },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList ? lst) => { watching = lst; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList ? lst) => { wish = lst; }) },
        };

        await service.ShowAsync<CollectionPopup>(null, parameters, Options(MaxWidth.Medium));
    }

    public static async Task CompleteListPopup(this IDialogService service, string? titleHead, WatchingList? watching, WishList? wish, HashSet<MediaDetail> items, string? culture)
    {
        ComponentActions<HashSet<MediaDetail>> actions = new(list => list == null || list.Empty());

        actions.StartLoading?.Invoke(null);

        var parameters = new DialogParameters<CompleteListPopup>
        {
            { x => x.TitleHead, titleHead },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.Culture, culture },
            { x => x.Items, items },
            { x => x.Actions, actions },
            { x => x.ItemsChanged, Factory.Create(new object(), (HashSet<MediaDetail> lst) => { items = lst; }) },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList ? lst) => { watching = lst; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList ? lst) => { wish = lst; }) },
        };

        await service.ShowAsync<CompleteListPopup>(titleHead, parameters, Options(MaxWidth.Large));

        actions.FinishLoading?.Invoke(items);
    }

    public static async Task MediaPopup(this IDialogService service, WatchingList? watching, WishList? wish, MediaType? type, string? tmdbId, string? culture)
    {
        var parameters = new DialogParameters<MediaPopup>
        {
            { x => x.TmdbId, tmdbId },
            { x => x.Type, type },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.Culture, culture },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList ? lst) => { watching = lst; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList ? lst) => { wish = lst; }) },
        };

        await service.ShowAsync<MediaPopup>(null, parameters, Options(MaxWidth.Large));
    }

    public static async Task MyWatchingListPopup(this IDialogService service, ComponentActions<WatchingList?> actions, MediaType type,
        WatchingList? watching, WishList? wish, string? culture)
    {
        actions.StartLoading?.Invoke(null);

        var parameters = new DialogParameters<MyWatchingListPopup>
        {
            { x => x.Actions, actions },
            { x => x.MediaType, type },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.Culture, culture },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }) },
        };

        var Title = type == MediaType.movie ? Translations.Module.Profile.MyMovieFollowing : Translations.Module.Profile.MySeriesFollowing;
        var Quantity = type == MediaType.movie ? watching?.Movies.Count ?? 0 : watching?.Shows.Count ?? 0;

        await service.ShowAsync<MyWatchingListPopup>(Title.Format(Quantity), parameters, Options(MaxWidth.Large));

        actions.FinishLoading?.Invoke(watching);
    }

    public static async Task MyWishListPopup(this IDialogService service, ComponentActions<WishList?> actions, WatchingList? watching, WishList? wish,
        MediaType type, string? culture)
    {
        actions.StartLoading?.Invoke(null);

        var parameters = new DialogParameters<MyWishListPopup>
        {
            { x => x.Actions, actions },
            { x => x.MediaType, type },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.Culture, culture },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }) },
        };

        var Title = type == MediaType.movie ? Translations.Module.Profile.MyMovieWishlist : Translations.Module.Profile.MySeriesWishlist;
        var Quantity = type == MediaType.movie ? wish?.Movies.Count ?? 0 : wish?.Shows.Count ?? 0;

        await service.ShowAsync<MyWishListPopup>(Title.Format(Quantity), parameters, Options(MaxWidth.Large));

        actions.FinishLoading?.Invoke(wish);
    }

    public static async Task AccountPopup(this IDialogService service)
    {
        var parameters = new DialogParameters<AccountPopup> { };

        await service.ShowAsync<AccountPopup>(Translations.Module.Auth.MyAccount, parameters, Options(MaxWidth.Small));
    }

    public static async Task PlatformPopup(this IDialogService service, ProviderModel? provider, WatchingList? watching, WishList? wish,
        string? watchRegion, string? providerId, string? culture)
    {
        var parameters = new DialogParameters<PlatformPopup>
        {
            { x => x.Provider, provider },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.Culture, culture },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }) },
            { x => x.WatchRegion, watchRegion },
            { x => x.ProviderId, providerId },
        };

        await service.ShowAsync<PlatformPopup>(provider?.name, parameters, Options(MaxWidth.Large));
    }

    public static async Task SelectItemsCollection(this IDialogService service, List<Collection> items, HashSet<string> selectedItems,
        EventCallback<HashSet<string>> itemsChanged)
    {
        var parameters = new DialogParameters<SelectItemsCollection>
        {
            { x => x.ItemsCollection, items },
            { x => x.SelectedItems, selectedItems },
            { x => x.SelectedItemsChanged, itemsChanged },
        };

        await service.ShowAsync<SelectItemsCollection>(Translations.Module.Profile.WhatHaveYouWatched, parameters, Options(MaxWidth.ExtraSmall));
    }

    public static async Task SeasonPopup(this IDialogService service, string? showTitle, string? showSeasonName, string? tmdbId, int? seasonNumber)
    {
        var parameters = new DialogParameters<SeasonPopup>
        {
            { x => x.TmdbId, tmdbId },
            { x => x.SeasonNumber, seasonNumber },
        };

        await service.ShowAsync<SeasonPopup>($"{showTitle} ({showSeasonName})", parameters, Options(MaxWidth.Medium));
    }

    public static async Task SettingsPopup(this IDialogService service)
    {
        await service.ShowAsync<SettingsPopup>(Translations.Module.Help.Settings, Options(MaxWidth.Small));
    }

    public static async Task SubscriptionPopup(this IDialogService service)
    {
        var parameters = new DialogParameters<SubscriptionPopup> { };

        await service.ShowAsync<SubscriptionPopup>(Translations.Module.Subscription.MySubscription, parameters, Options(MaxWidth.Medium));
    }

    public static async Task OnboardingPopup(this IDialogService service, string culture)
    {
        var parameters = new DialogParameters<Onboarding>
        {
            { x => x.Culture, culture },
        };

        await service.ShowAsync<Onboarding>(string.Format(Translations.Module.Help.WelcomeTo, AppInfo.Title), parameters, Options(MaxWidth.Medium));
    }

    public static async Task AskReviewPopup(this IDialogService service)
    {
        await service.ShowAsync<AskReview>(string.Format(Translations.Module.Help.WriteReviewTitle, AppInfo.Title), Options(MaxWidth.Small, false, false));
    }

    public static DialogOptions Options(MaxWidth width, bool allowClose = true, bool showHeader = true)
    {
        return new DialogOptions
        {
            CloseOnEscapeKey = allowClose,
            CloseButton = allowClose,
            BackdropClick = allowClose,
            NoHeader = !showHeader,
            Position = DialogPosition.Center,
            MaxWidth = width
        };
    }
}
