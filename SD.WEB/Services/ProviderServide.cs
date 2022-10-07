using Blazored.SessionStorage;
using SD.Shared.Modal;
using SD.WEB.Core;
using System.Diagnostics.CodeAnalysis;

namespace SD.WEB.Services
{
    public class ProviderServide
    {
        private List<Provider> _providers = new();

        public string BaseApi([NotNullWhen(true)] HttpClient http) => http.BaseAddress?.ToString().Contains("localhost") ?? true ? "http://localhost:7071/api/" : http.BaseAddress.ToString() + "api/";

        public async Task<List<Provider>> GetAllProviders(HttpClient Http, ISyncSessionStorageService session)
        {
            if (!_providers.Any())
            {
                _providers = (await Http.Get<List<Provider>>(BaseApi(Http) + "Provider/GetAll", session));
            }

            return _providers.OrderBy(o => o.priority).ToList();
        }

        public async Task SaveProvider(HttpClient Http, ISyncSessionStorageService session, Provider provider)
        {
            var temp = _providers.Single(s => s.id == provider.id);

            temp = provider;

            await Http.Post("Provider/Post", _providers, session, "Data/providers.json");
        }

        public async Task UpdateAllProvider(HttpClient Http)
        {
            await Http.Put<AllProviders>("Provider/UpdateAllProvider", null);
        }
    }
}