using Blazored.LocalStorage;
using SD.Shared.Modal;
using SD.WEB.Core;

namespace SD.WEB.Services
{
    public class ProviderServide
    {
        private List<Provider> _providers = new();

        public async Task<List<Provider>> GetAllProviders(HttpClient Http, ISyncLocalStorageService session)
        {
            if (!_providers.Any())
            {
                _providers = (await Http.Get<AllProviders>(session, "Provider/GetAll")).Items;
            }

            return _providers.OrderBy(o => o.priority).ToList();
        }

        public async Task SaveProvider(HttpClient Http, ISyncLocalStorageService session, Provider provider)
        {
            var temp = _providers.Single(s => s.id == provider.id);

            temp = provider;

            await Http.Post("Provider/Post", _providers, session, "Data/providers.json");
        }

        public async Task UpdateAllProvider(HttpClient Http)
        {
            await Http.Put("Provider/UpdateAllProvider");
        }
    }
}