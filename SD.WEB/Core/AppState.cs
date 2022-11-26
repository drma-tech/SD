namespace SD.WEB.Core
{
    public class AppState
    {
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
    }
}