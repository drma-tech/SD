using Blazored.SessionStorage;
using SD.Shared.Modal;
using SD.WEB.Core;

namespace SD.WEB.Api
{
    public static class AllProvidersApi
    {
        private struct Endpoint
        {
            public const string GetAll = "Public/Provider/GetAll";
            public const string Post = "Provider/Post";
            public const string Sync = "Provider/SyncProviders";
        }

        public static async Task<AllProviders?> Provider_GetAll(this HttpClient http, ISyncSessionStorageService? storage)
        {
            return await http.Get<AllProviders>(Endpoint.GetAll, false, storage);
        }

        public static async Task<HttpResponseMessage> Provider_Post(this HttpClient http, AllProviders? obj, ISyncSessionStorageService? storage)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await http.Post(Endpoint.Post, false, obj, storage, Endpoint.GetAll);
        }

        public static async Task<HttpResponseMessage> Provider_Sync(this HttpClient http, ISyncSessionStorageService? storage)
        {
            return await http.Put<AllProviders>(Endpoint.Sync, false, null, storage, Endpoint.GetAll);
        }
    }
}