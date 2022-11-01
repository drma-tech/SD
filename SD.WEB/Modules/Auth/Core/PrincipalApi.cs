using Blazored.SessionStorage;
using SD.Shared.Model.Auth;

namespace SD.WEB.Modules.Auth.Core
{
    public static class PrincipalApi
    {
        private struct PrincipalEndpoint
        {
            public const string Get = "Principal/Get";
            public const string Add = "Principal/Add";
        }

        public static async Task<ClientePrincipal?> Principal_Get(this HttpClient http, ISyncSessionStorageService? storage)
        {
            return await http.Get<ClientePrincipal>(PrincipalEndpoint.Get, false, storage);
        }

        public static async Task<HttpResponseMessage> Principal_Add(this HttpClient http, ClientePrincipal? obj, ISyncSessionStorageService? storage)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await http.Post(PrincipalEndpoint.Add, false, obj, storage, PrincipalEndpoint.Get);
        }
    }
}