using Microsoft.AspNetCore.Components.Authorization;

namespace SD.WEB.Core
{
    public class AppState
    {
        #region PROFILE DATA

        public WishList? WishList { get; private set; }
        public WatchedList? WatchedList { get; private set; }
        public WatchingList? WatchingList { get; private set; }

        public Action? WishListChanged { get; set; }
        public Action? WatchedListChanged { get; set; }
        public Action? WatchingListChanged { get; set; }

        public void ChangeWishList(WishList? list)
        {
            WishList = list;
            WishListChanged?.Invoke();
        }

        public void ChangeWatchedList(WatchedList? list)
        {
            WatchedList = list;
            WatchedListChanged?.Invoke();
        }

        public void ChangeWatchingList(WatchingList? list)
        {
            WatchingList = list;
            WatchingListChanged?.Invoke();
        }

        #endregion PROFILE DATA

        #region USER SESSION

        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public AppState(AuthenticationStateProvider authenticationStateProvider)
        {
            AuthenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> IsUserAuthenticated(AuthenticationState? authState = null)
        {
            authState ??= await AuthenticationStateProvider.GetAuthenticationStateAsync();

            return authState.User.Identity != null && authState.User.Identity.IsAuthenticated;
        }

        public async Task<string?> GetIdUser(AuthenticationState? authState = null)
        {
            authState ??= await AuthenticationStateProvider.GetAuthenticationStateAsync();

            return authState.User.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        #endregion USER SESSION
    }
}