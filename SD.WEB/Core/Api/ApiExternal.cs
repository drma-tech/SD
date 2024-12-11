namespace SD.WEB.Core.Api
{
    public abstract class ApiExternal(IHttpClientFactory factory) : ApiCore(factory)
    {
        protected async Task<T?> GetAsync<T>(string uri) where T : class
        {
            return await base.GetAsync<T>(uri, false);
        }
    }
}