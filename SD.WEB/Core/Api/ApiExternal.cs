namespace SD.WEB.Core.Api;

public abstract class ApiExternal(IHttpClientFactory factory) : ApiCore(factory, null)
{
    protected string BaseEndpoint => Http.BaseAddress?.ToString().Contains("localhost") ?? true
        ? "http://localhost:7071/api/"
        : $"{Http.BaseAddress}api/";
}