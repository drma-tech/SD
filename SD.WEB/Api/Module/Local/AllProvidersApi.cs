using SD.WEB.Api.Core;
using System.Net.Http.Json;

namespace SD.WEB.Api.Module.Local;

public class AllProvidersApi(IHttpClientFactory factory) : ApiLocal(factory)
{
    public async Task<AllProviders?> GetAll(RenderControlState<AllProviders?>? state, CancellationToken cancellationToken)
    {
        if (state != null) await state.StartLoading(null);
        var result = await LocalHttp.GetFromJsonAsync("/data/providers.json", JavascriptContext.Default.AllProviders, cancellationToken);
        if (state != null) await state.FinishLoading(result);
        return result;
    }
}