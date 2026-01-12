using System.Net.Http.Json;

namespace SD.WEB.Modules.Platform.Core;

public class AllProvidersApi(IHttpClientFactory factory) : ApiCore(factory, null, ApiType.Local)
{
    public async Task<AllProviders?> GetAll()
    {
        return await LocalHttp.GetFromJsonAsync<AllProviders>("/data/providers.json");
    }
}