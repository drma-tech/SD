using Microsoft.AspNetCore.Components;
using MudBlazor;
using SD.WEB.Modules.Auth;
using SD.WEB.Modules.Collections.Components;
using SD.WEB.Modules.Platform.Components;
using SD.WEB.Modules.Profile.Components;
using SD.WEB.Modules.Subscription.Components;
using SD.WEB.Modules.Support;
using SD.WEB.Shared;
using SD.WEB.Shared.Core;

namespace SD.WEB.Core.Helper;

public static class PopupHelper
{
    public static readonly EventCallbackFactory Factory = new();

    public static async Task CollectionPopup(this IDialogService service, WatchedList? watched, WatchingList? watching, WishList? wish, MediaType? type, string? collectionId)
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
        };

        await service.ShowAsync<CollectionPopup>(null, parameters, Options(MaxWidth.Medium));
    }

    public static async Task CompleteListPopup(this IDialogService service, string? titleHead, WatchedList? watched, WatchingList? watching, WishList? wish, HashSet<MediaDetail> items)
    {
        ComponentActions<HashSet<MediaDetail>> actions = new(list => list == null || list.Empty());

        var parameters = new DialogParameters<CompleteListPopup>
        {
            { x => x.TitleHead, titleHead },
            { x => x.Watched, watched },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.Items, items },
            { x => x.Actions, actions },
            { x => x.ItemsChanged, Factory.Create(new object(), (HashSet<MediaDetail> lst) => { items = lst; }) },
            { x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? lst) => { watched = lst; }) },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList ? lst) => { watching = lst; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList ? lst) => { wish = lst; }) },
        };

        await service.ShowAsync<CompleteListPopup>(titleHead, parameters, Options(MaxWidth.Large));

        actions.FinishLoading?.Invoke(items);
    }

    public static async Task MediaPopup(this IDialogService service, WatchedList? watched, WatchingList? watching, WishList? wish, MediaType? type, string? tmdbId)
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
        };

        await service.ShowAsync<MediaPopup>(null, parameters, Options(MaxWidth.Large));
    }

    public static async Task MySuggestionsPopup(this IDialogService service, SD.Shared.Models.MySuggestions? mySuggestions,
        EventCallback<SD.Shared.Models.MySuggestions>? mySuggestionsChanged)
    {
        var parameters = new DialogParameters<MySuggestionsPopup>
        {
            { x => x.MySuggestions, mySuggestions },
            { x => x.MySuggestionsChanged, mySuggestionsChanged },
        };

        await service.ShowAsync<MySuggestionsPopup>(GlobalTranslations.SuggestionFilters, parameters, Options(MaxWidth.Small));
    }

    public static async Task MyWatchingListPopup(this IDialogService service, ComponentActions<WatchingList?> actions, MediaType type,
        WatchedList? watched, WatchingList? watching, WishList? wish)
    {
        var parameters = new DialogParameters<MyWatchingListPopup>
        {
            { x => x.Actions, actions },
            { x => x.MediaType, type },
            { x => x.Watched, watched },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }) },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }) },
        };

        var Title = type == MediaType.movie ? Modules.Profile.Resources.Translations.MyMovieWatching : Modules.Profile.Resources.Translations.MyShowWatching;
        var Quantity = type == MediaType.movie ? watching?.Movies.Count ?? 0 : watching?.Shows.Count ?? 0;

        await service.ShowAsync<MyWatchingListPopup>(Title.Format(Quantity), parameters, Options(MaxWidth.Large));

        actions.FinishLoading?.Invoke(watching);
    }

    public static async Task MyWishListPopup(this IDialogService service, ComponentActions<WishList?> actions, WatchedList? watched, WatchingList? watching, WishList? wish,
        MediaType type)
    {
        var parameters = new DialogParameters<MyWishListPopup>
        {
            { x => x.Actions, actions },
            { x => x.MediaType, type },
            { x => x.Watched, watched },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }) },
            { x => x.WatchingChanged, Factory.Create(new object(), (WatchingList? list) => { watching = list; }) },
            { x => x.WishChanged, Factory.Create(new object(), (WishList? list) => { wish = list; }) },
        };

        var Title = type == MediaType.movie ? Modules.Profile.Resources.Translations.MyMovieWishlist : Modules.Profile.Resources.Translations.MyShowWishlist;
        var Quantity = type == MediaType.movie ? wish?.Movies.Count ?? 0 : wish?.Shows.Count ?? 0;

        await service.ShowAsync<MyWishListPopup>(Title.Format(Quantity), parameters, Options(MaxWidth.Large));

        actions.FinishLoading?.Invoke(wish);
    }

    public static async Task AccountPopup(this IDialogService service)
    {
        var parameters = new DialogParameters<AccountPopup> { };

        await service.ShowAsync<AccountPopup>(Modules.Auth.Resources.Translations.MyAccount, parameters, Options(MaxWidth.Small));
    }

    public static async Task PlatformPopup(this IDialogService service, ProviderModel? provider, WatchedList? watched, WatchingList? watching, WishList? wish,
        string? watchRegion, string? providerId)
    {
        var parameters = new DialogParameters<PlatformPopup>
        {
            { x => x.Provider, provider },
            { x => x.Watched, watched },
            { x => x.Watching, watching },
            { x => x.Wish, wish },
            { x => x.WatchedChanged, Factory.Create(new object(), (WatchedList? list) => { watched = list; }) },
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
            { x => x.SelectedItemsChanged, itemsChanged }
        };

        await service.ShowAsync<SelectItemsCollection>(GlobalTranslations.WhatHaveYouWatched, parameters, Options(MaxWidth.ExtraSmall));
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
        await service.ShowAsync<SettingsPopup>(GlobalTranslations.Settings, Options(MaxWidth.Small));
    }

    public static async Task SubscriptionPopup(this IDialogService service)
    {
        var parameters = new DialogParameters<SubscriptionPopup> { };

        await service.ShowAsync<SubscriptionPopup>(Modules.Subscription.Resources.Translations.MySubscription, parameters, Options(MaxWidth.Medium));
    }

    public static async Task OnboardingPopup(this IDialogService service)
    {
        await service.ShowAsync<Onboarding>(string.Format(GlobalTranslations.WelcomeTo, SeoTranslations.AppName), Options(MaxWidth.Medium));
    }

    public static async Task AskReviewPopup(this IDialogService service)
    {
        await service.ShowAsync<AskReview>(string.Format(GlobalTranslations.WriteReviewTitle, SeoTranslations.AppName), Options(MaxWidth.Small));
    }

    public static async Task LoginPopup(this IDialogService service)
    {
        await service.ShowAsync<LoginPopup>("Log in or sign up", Options(MaxWidth.ExtraSmall));
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