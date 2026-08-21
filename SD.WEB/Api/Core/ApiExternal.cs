namespace SD.WEB.Api.Core;

public abstract class ApiExternal(IHttpClientFactory factory) : ApiCore(key: null, extraKeys: [])
{
    protected HttpClient AnonymousHttp => factory.CreateClient("Anonymous");

    protected async Task<string?> GetStringAsync(string endpoint, CancellationToken cancellationToken)
    {
        return await GetStringAsync(AnonymousHttp, endpoint, cancellationToken);
    }

    protected async Task<T?> GetAsync<T>(string uri, bool setNewVersion, RenderControlState<T>? state, CancellationToken cancellationToken) where T : class
    {
        return await GetAsync(AnonymousHttp, $"public/external?url=" + uri.ConvertFromStringToBase64(), setNewVersion, state, cancellationToken);
    }
}