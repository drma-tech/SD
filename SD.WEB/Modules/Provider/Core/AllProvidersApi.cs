using System.Net.Http.Json;
using SD.WEB.Shared;

namespace SD.WEB.Modules.Provider.Core;

public class AllProvidersApi(IHttpClientFactory factory) : ApiCore(factory, null)
{
    public async Task<AllProviders?> GetAll(RenderControlCore<AllProviders?>? core)
    {
        core?.LoadingStarted?.Invoke();
        var result = new AllProviders();
        try
        {
            result = await _http.GetFromJsonAsync<AllProviders>("/data/providers.json");
            return result;
        }
        finally
        {
            core?.LoadingFinished?.Invoke(result);
        }
    }
}