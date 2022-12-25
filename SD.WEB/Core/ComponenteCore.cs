using Blazorise;
using BlazorPro.BlazorSize;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SD.WEB.Modules.Auth.Core;
using SD.WEB.Modules.Profile.Core;

namespace SD.WEB.Core
{
    public abstract class ComponenteNoDataCore<T> : ComponentBase where T : class
    {
        [Inject] protected ILogger<T> Logger { get; set; } = default!;
        [Inject] protected INotificationService Toast { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] protected WatchedListApi WatchedListApi { get; set; } = default!;
        [Inject] protected WishListApi WishListApi { get; set; } = default!;
        [Inject] protected WatchingListApi WatchingListApi { get; set; } = default!;
        [Inject] protected AppState AppState { get; set; } = default!;
        [Inject] protected IResizeListener listener { get; set; } = default!;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                listener.OnResized += WindowResized;
            }
        }

        private async void WindowResized(object obj, BrowserWindowSize window)
        {
            AppStateStatic.OnMobile = await listener.MatchMedia(Breakpoints.XSmallDown);
            AppStateStatic.OnTablet = await listener.MatchMedia(Breakpoints.SmallUp);
            AppStateStatic.OnDesktop = await listener.MatchMedia(Breakpoints.MediumUp);
            AppStateStatic.OnWidescreen = await listener.MatchMedia(Breakpoints.LargeUp);
            AppStateStatic.OnFullHD = await listener.MatchMedia(Breakpoints.XLargeUp);

            StateHasChanged();
        }
    }

    /// <summary>
    /// if you implement the OnInitializedAsync method, call 'await base.OnInitializedAsync();'
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ComponenteCore<T> : ComponenteNoDataCore<T> where T : class
    {
        protected abstract Task LoadData();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    await LoadData();
                }
            }
            catch (Exception ex)
            {
                ex.ProcessException(Toast, Logger);
            }
        }
    }

    /// <summary>
    /// if you implement the OnInitializedAsync method, call 'await base.OnInitializedAsync();'
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PageCore<T> : ComponenteCore<T> where T : class
    {
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected PrincipalApi PrincipalApi { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (await AppState.IsUserAuthenticated())
                {
                    var principal = await PrincipalApi.Get();

                    if (principal == null) //force the registration, if the main account does not exist yet
                    {
                        Navigation.NavigateTo("/ProfilePrincipal");
                    }

                    AppState.ChangeWatchingList(await WatchingListApi.Get(await AppState.IsUserAuthenticated()));
                    AppState.ChangeWishList(await WishListApi.Get(await AppState.IsUserAuthenticated()));
                    AppState.ChangeWatchedList(await WatchedListApi.Get(await AppState.IsUserAuthenticated()));
                }
            }
            catch (Exception ex)
            {
                ex.ProcessException(Toast, Logger);
            }
        }
    }

    public abstract class PageNoDataCore<T> : ComponenteNoDataCore<T> where T : class
    {
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected PrincipalApi PrincipalApi { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (await AppState.IsUserAuthenticated())
                {
                    var principal = await PrincipalApi.Get();

                    if (principal == null) //force the registration, if the main account does not exist yet
                    {
                        Navigation.NavigateTo("/ProfilePrincipal");
                    }

                    AppState.ChangeWatchingList(await WatchingListApi.Get(await AppState.IsUserAuthenticated()));
                    AppState.ChangeWishList(await WishListApi.Get(await AppState.IsUserAuthenticated()));
                    AppState.ChangeWatchedList(await WatchedListApi.Get(await AppState.IsUserAuthenticated()));
                }
            }
            catch (Exception ex)
            {
                ex.ProcessException(Toast, Logger);
            }
        }
    }
}