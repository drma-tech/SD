using System.Net.Http.Json;

namespace SD.WEB.Modules.Platform.Core;

public class AllProvidersApi(IHttpClientFactory factory) : ApiCore(factory, null, ApiType.Local)
{
    public async Task<AllProviders?> GetAll(CancellationToken cancellationToken)
    {
        return await LocalHttp.GetFromJsonAsync("/data/providers.json", JavascriptContext.Default.AllProviders, cancellationToken);
    }
}