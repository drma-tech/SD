namespace SD.WEB.Modules.Provider.Core
{
    public static class AllProvidersApi
    {
        private struct Endpoint
        {
            public const string GetAll = "Public/Provider/GetAll";
            public const string Post = "Provider/Post";
            public const string Sync = "Provider/SyncProviders";
        }

        public static async Task<AllProviders?> Provider_GetAll(this HttpClient http)
        {
            return await http.Get<AllProviders>(Endpoint.GetAll, false);
        }

        public static async Task<HttpResponseMessage> Provider_Post(this HttpClient http, AllProviders? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await http.Post(Endpoint.Post, false, obj, Endpoint.GetAll);
        }

        public static async Task<HttpResponseMessage> Provider_Sync(this HttpClient http)
        {
            return await http.Put<AllProviders>(Endpoint.Sync, false, null, Endpoint.GetAll);
        }
    }
}