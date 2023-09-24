using Microsoft.AspNetCore.Components.Authorization;
using SD.WEB.Modules.Profile.Core;

namespace SD.WEB.Core
{
    public class AppState
    {
        public AppState(AuthenticationStateProvider authenticationStateProvider, WatchedListApi watchedListApi, WatchingListApi watchingListApi, WishListApi wishListApi)
        {
            AuthenticationStateProvider = authenticationStateProvider;
            WatchedListApi = watchedListApi;
            WatchingListApi = watchingListApi;
            WishListApi = wishListApi;
        }

        #region USER SESSION

        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private bool? isUserAuthenticated { get; set; }
        private string? userId { get; set; }

        public async Task<bool> IsUserAuthenticated(AuthenticationState? authState = null)
        {
            if (!isUserAuthenticated.HasValue)
            {
                authState ??= await AuthenticationStateProvider.GetAuthenticationStateAsync();

                isUserAuthenticated = authState.User.Identity != null && authState.User.Identity.IsAuthenticated;
            }

            return isUserAuthenticated.Value;
        }

        public async Task<string?> GetIdUser(AuthenticationState? authState = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                authState ??= await AuthenticationStateProvider.GetAuthenticationStateAsync();

                userId = authState.User.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            }

            return userId;
        }

        #endregion USER SESSION

        #region PROFILE DATA

        private readonly WatchedListApi WatchedListApi;
        private readonly WishListApi WishListApi;
        private readonly WatchingListApi WatchingListApi;

        private WatchedList? WatchedList;
        private WatchingList? WatchingList;
        private WishList? WishList;

        public Action<WatchedList?>? WatchedListChanged { get; set; }
        public Action<WatchingList?>? WatchingListChanged { get; set; }
        public Action<WishList?>? WishListChanged { get; set; }

        public async Task<WatchedList?> GetWatchedList(bool privatePage)
        {
            var authenticated = await IsUserAuthenticated();

            if (privatePage && !authenticated) throw new InvalidOperationException("user not authenticated");

            if (WatchedList == null)
            {
                ChangeWatchedList(await WatchedListApi.Get(authenticated));
            }

            return WatchedList;
        }

        public async Task<WatchingList?> GetWatchingList(bool privatePage)
        {
            var authenticated = await IsUserAuthenticated();

            if (privatePage && !authenticated) throw new InvalidOperationException("user not authenticated");

            if (WatchingList == null)
            {
                ChangeWatchingList(await WatchingListApi.Get(authenticated));
            }

            return WatchingList;
        }

        public async Task<WishList?> GetWishList(bool privatePage)
        {
            var authenticated = await IsUserAuthenticated();

            if (privatePage && !authenticated) throw new InvalidOperationException("user not authenticated");

            if (WishList == null)
            {
                ChangeWishList(await WishListApi.Get(authenticated));
            }

            return WishList;
        }

        public void ChangeWatchedList(WatchedList? list)
        {
            WatchedList = list;
            WatchedListChanged?.Invoke(list);
        }

        public void ChangeWatchingList(WatchingList? list)
        {
            WatchingList = list;
            WatchingListChanged?.Invoke(list);
        }

        public void ChangeWishList(WishList? list)
        {
            WishList = list;
            WishListChanged?.Invoke(list);
        }

        #endregion PROFILE DATA
    }
}