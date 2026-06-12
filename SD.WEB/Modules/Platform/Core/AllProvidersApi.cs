using System.Net.Http.Json;

namespace SD.WEB.Modules.Platform.Core;

public class AllProvidersApi(IHttpClientFactory factory) : ApiCore(factory, null, ApiType.Local)
{
    public async Task<AllProviders?> GetAll(ComponentActions<AllProviders?>? actions, CancellationToken cancellationToken)
    {
        if (actions != null) await actions.StartLoading(null);
        var result = await LocalHttp.GetFromJsonAsync("/data/providers.json", JavascriptContext.Default.AllProviders, cancellationToken);
        if (actions != null) await actions.FinishLoading(result);
        return result;
    }
}