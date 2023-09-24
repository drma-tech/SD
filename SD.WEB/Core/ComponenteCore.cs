using Blazorise;
using BlazorPro.BlazorSize;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SD.WEB.Modules.Auth.Core;

namespace SD.WEB.Core
{
    /// <summary>
    /// if you implement the OnAfterRenderAsync method, call 'await base.OnAfterRenderAsync(firstRender);'
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ComponenteCore<T> : ComponentBase where T : class
    {
        [Inject] protected ILogger<T> Logger { get; set; } = default!;
        [Inject] protected INotificationService Toast { get; set; } = default!;
        [Inject] protected IResizeListener listener { get; set; } = default!;

        [Inject] protected AppState AppState { get; set; } = default!; //TODO: remove from here

        public new void StateHasChanged()
        {
            base.StateHasChanged();
        }

        /// <summary>
        /// Use this method if you need to access javascript or data of logged user.
        /// </summary>
        /// <returns></returns>
        protected virtual Task LoadDataRender()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// if you implement the OnAfterRenderAsync method, call 'await base.OnAfterRenderAsync(firstRender);'
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await base.OnAfterRenderAsync(firstRender);

                if (firstRender)
                {
                    listener.OnResized += WindowResized;

                    await LoadDataRender();
                }
            }
            catch (Exception ex)
            {
                ex.ProcessException(Toast, Logger);
            }
        }

        private async void WindowResized(object? obj, BrowserWindowSize window)
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
    /// if you implement the OnAfterRenderAsync method, call 'await base.OnAfterRenderAsync(firstRender);'
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PageCore<T> : ComponenteCore<T> where T : class
    {
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected PrincipalApi PrincipalApi { get; set; } = default!;

        /// <summary>
        /// if you implement the OnAfterRenderAsync method, call 'await base.OnAfterRenderAsync(firstRender);'
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await base.OnAfterRenderAsync(firstRender);

                if (await AppState.IsUserAuthenticated())
                {
                    var principal = await PrincipalApi.Get();

                    if (principal == null) //force the registration, if the main account does not exist yet
                    {
                        Navigation.NavigateTo("/ProfilePrincipal");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ProcessException(Toast, Logger);
            }
        }
    }
}