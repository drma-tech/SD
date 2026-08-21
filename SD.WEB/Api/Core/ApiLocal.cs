namespace SD.WEB.Api.Core;

public abstract class ApiLocal(IHttpClientFactory factory) : ApiCore(key: null, extraKeys: [])
{
    protected HttpClient LocalHttp => factory.CreateClient("Local");
}