using Microsoft.Extensions.Caching.Memory;
using SD.WEB.Shared;

namespace SD.WEB.Core.Api
{
    public abstract class ApiCosmos<T>(IHttpClientFactory factory, IMemoryCache memoryCache, object? cacheKey) : ApiCore(factory) where T : class
    {
        public Action<T?>? DataChanged { get; set; }

        protected CacheSettings? CacheSettings { get; set; } = new CacheSettings();
        protected IMemoryCache _cache { get; set; } = memoryCache;
        protected object? cacheKey { get; set; } = cacheKey;

        private string baseEndpoint => _http.BaseAddress?.ToString().Contains("localhost") ?? true ? "http://localhost:7071/api/" : $"{_http.BaseAddress}api/";

        protected void CleanCache()
        {
            if (cacheKey != null) _cache.Remove(cacheKey);
        }

        protected async Task<T?> GetAsync(string endpoint, RenderControlCore<T?>? core, object? customCacheKey = null)
        {
            var localKey = (customCacheKey ?? cacheKey) ?? throw new NotificationException("localKey null");

            core?.LoadingStarted?.Invoke();

            var result = _cache.Get<T>(localKey);

            try
            {
                if (result == null)
                {
                    result = await GetAsync<T>($"{baseEndpoint}{endpoint}");

                    _cache.Set(localKey, result, CacheSettings);
                }

                return result;
            }
            finally
            {
                core?.LoadingFinished?.Invoke(result);
            }
        }

        protected async Task<string?> GetValueAsync(string endpoint, RenderControlCore<string?>? core, object? customCacheKey = null)
        {
            var localKey = (customCacheKey ?? cacheKey) ?? throw new NotificationException("localKey null");

            core?.LoadingStarted?.Invoke();

            var result = _cache.Get<string>(localKey);

            try
            {
                if (result == null)
                {
                    result = await GetValueAsync($"{baseEndpoint}{endpoint}");

                    _cache.Set(localKey, result, CacheSettings);
                }

                return result;
            }
            finally
            {
                core?.LoadingFinished?.Invoke(result);
            }
        }

        protected async Task<HashSet<T>> GetListAsync(string endpoint, RenderControlCore<HashSet<T>>? core, object? customCacheKey = null)
        {
            var localKey = (customCacheKey ?? cacheKey) ?? throw new NotificationException("localKey null");

            core?.LoadingStarted?.Invoke();

            var result = _cache.Get<HashSet<T>>(localKey);

            try
            {
                if (result == null)
                {
                    result = await GetListAsync<T>($"{baseEndpoint}{endpoint}");

                    _cache.Set(localKey, result, CacheSettings);
                }

                return result ?? [];
            }
            finally
            {
                core?.LoadingFinished?.Invoke(result);
            }
        }

        protected async Task<T?> PostAsync(string endpoint, RenderControlCore<T?>? core, T? obj)
        {
            return await PostAsync<T>(endpoint, core, obj);
        }

        protected async Task<T?> PostAsync<I>(string endpoint, RenderControlCore<T?>? core, I? obj) where I : class
        {
            T? result = default;

            try
            {
                core?.ProcessingStarted?.Invoke();

                result = await PostAsync<I, T>($"{baseEndpoint}{endpoint}", obj);

                if (cacheKey != null && result != default) _cache.Set(cacheKey, result, CacheSettings);

                DataChanged?.Invoke(result);

                return result;
            }
            finally
            {
                core?.ProcessingFinished?.Invoke(result);
            }
        }

        protected async Task<T?> PutAsync<I>(string endpoint, RenderControlCore<T?>? core, I? obj) where I : class
        {
            T? result = default;

            try
            {
                core?.ProcessingStarted?.Invoke();

                result = await PutAsync<I, T>($"{baseEndpoint}{endpoint}", obj);

                if (cacheKey != null && result != default) _cache.Set(cacheKey, result, CacheSettings);

                DataChanged?.Invoke(result);

                return result;
            }
            finally
            {
                core?.ProcessingFinished?.Invoke(result);
            }
        }

        protected async Task<T?> DeleteAsync(string endpoint, RenderControlCore<T?>? core)
        {
            T? result = default;

            try
            {
                core?.ProcessingStarted?.Invoke();

                result = await DeleteAsync<T>($"{baseEndpoint}{endpoint}");

                if (cacheKey != null) _cache.Remove(cacheKey);

                DataChanged?.Invoke(result);

                return result;
            }
            finally
            {
                core?.ProcessingFinished?.Invoke(result);
            }
        }
    }
}