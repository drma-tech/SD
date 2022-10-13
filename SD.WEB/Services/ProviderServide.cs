using Blazored.SessionStorage;
using SD.Shared.Modal;
using SD.WEB.Core;

namespace SD.WEB.Services
{
    public class ProviderServide
    {
        private List<ProviderModel> _providers = new();

        public async Task<List<ProviderModel>> GetAllProviders(HttpClient Http, ISyncSessionStorageService session)
        {
            if (!_providers.Any())
            {
                _providers = await Http.GetList<ProviderModel>("Public/Provider/GetAll", false, session);
            }

            return _providers.OrderBy(o => o.priority).ToList();
        }

        public async Task SaveProvider(HttpClient Http, ISyncSessionStorageService session, ProviderModel provider)
        {
            var temp = _providers.Single(s => s.id == provider.id);

            temp = provider;

            await Http.Post("Provider/Post", false, _providers, session, "Data/providers.json");
        }

        public async Task UpdateAllProvider(HttpClient Http)
        {
            await Http.Put<AllProviders>("Provider/UpdateAllProvider", false, null);
        }
    }
}