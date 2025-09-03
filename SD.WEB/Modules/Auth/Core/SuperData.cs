using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core
{
    public class SuperData
    {
        public AuthPrincipal? Principal { get; set; }
        public AuthLogin? Login { get; set; }
        public MyProviders? Providers { get; set; }
        public MySuggestions? Suggestions { get; set; }
        public WatchedList? WatchedList { get; set; }
        public WatchingList? WatchingList { get; set; }
        public WishList? WishList { get; set; }
    }
}