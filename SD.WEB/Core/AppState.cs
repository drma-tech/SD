using Microsoft.AspNetCore.Components.Authorization;

namespace SD.WEB.Core
{
    public class AppState
    {
        #region PROFILE DATA

        public WishList? WishList { get; private set; }
        public WatchedList? WatchedList { get; private set; }

        public Action? WishListChanged { get; set; }
        public Action? WatchedListChanged { get; set; }

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

        #endregion PROFILE DATA

        #region USER SESSION

        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public AppState(AuthenticationStateProvider authenticationStateProvider)
        {
            AuthenticationStateProvider = authenticationStateProvider;
        }

        private bool? Authenticated { get; set; }
        private string? User { get; set; }

        public async Task<bool> IsUserAuthenticated()
        {
            if (Authenticated.HasValue)
            {
                return Authenticated.Value;
            }
            else
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                Authenticated = user.Identity != null && user.Identity.IsAuthenticated;
                User = user.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                return Authenticated.Value;
            }
        }

        public async Task<string?> GetIdUser()
        {
            if (Authenticated.HasValue)
            {
                return User;
            }
            else
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                Authenticated = user.Identity != null && user.Identity.IsAuthenticated;
                User = user.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                return User;
            }
        }

        #endregion USER SESSION
    }
}