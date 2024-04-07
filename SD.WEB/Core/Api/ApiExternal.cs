using SD.WEB.Shared;

namespace SD.WEB.Core.Api
{
    public abstract class ApiExternal(IHttpClientFactory factory) : ApiCore(factory)
    {
        protected async Task<T?> GetAsync<T>(string uri, RenderControlCore<T?>? core) where T : class
        {
            core?.LoadingStarted?.Invoke();

            T? result = null;

            try
            {
                result = await GetAsync<T>(uri);

                return result;
            }
            finally
            {
                core?.LoadingFinished?.Invoke(result);
            }
        }

        protected async Task<string?> GetValueAsync(string uri, RenderControlCore<string?>? core)
        {
            core?.LoadingStarted?.Invoke();

            string? result = null;

            try
            {
                result = await GetValueAsync(uri);

                return result;
            }
            finally
            {
                core?.LoadingFinished?.Invoke(result);
            }
        }

        protected async Task<HashSet<T>> GetListAsync<T>(string uri, RenderControlCore<HashSet<T>>? core) where T : class
        {
            core?.LoadingStarted?.Invoke();

            HashSet<T> result = [];

            try
            {
                result = await GetListAsync<T>(uri);

                return result ?? [];
            }
            finally
            {
                core?.LoadingFinished?.Invoke(result);
            }
        }
    }
}