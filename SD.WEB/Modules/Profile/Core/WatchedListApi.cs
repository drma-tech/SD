using Blazored.SessionStorage;

namespace SD.WEB.Modules.Profile.Core
{
    public static class WatchedListApi
    {
        private struct Endpoint
        {
            public const string Get = "WatchedList/Get";
            public const string Post = "WatchedList/Post";
        }

        public static async Task<WatchedList?> WatchedList_Get(this HttpClient http, ISyncSessionStorageService? storage)
        {
            if (ComponenteUtils.IsAuthenticated)
            {
                return await http.Get<WatchedList>(Endpoint.Get, false, storage);
            }
            else
            {
                return new();
            }
        }

        public static async Task<HttpResponseMessage> WatchedList_Post(this HttpClient http, WatchedList? obj, ISyncSessionStorageService? storage)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await http.Post(Endpoint.Post, false, obj, storage, Endpoint.Get);
        }
    }
}