namespace SD.WEB.Modules.Profile.Core
{
    public static class WishListApi
    {
        private struct Endpoint
        {
            public const string Get = "WishList/Get";
            public const string Post = "WishList/Post";
        }

        public static async Task<WishList?> WishList_Get(this HttpClient http)
        {
            if (ComponenteUtils.IsAuthenticated)
            {
                return await http.Get<WishList>(Endpoint.Get, false);
            }
            else
            {
                return new();
            }
        }

        public static async Task<HttpResponseMessage> WishList_Post(this HttpClient http, WishList? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await http.Post(Endpoint.Post, false, obj, Endpoint.Get);
        }
    }
}