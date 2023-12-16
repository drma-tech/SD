using Microsoft.Extensions.Caching.Memory;
using SD.WEB.Shared;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SD.WEB.Core
{
    public abstract class ApiCore<T>
    {
        public CacheSettings? CacheSettings { get; set; } = new CacheSettings();
        public Action<T?>? DataChanged { get; set; }

        protected HttpClient _http { get; set; }
        protected IMemoryCache _cache { get; set; }

        private string? baseEndpoint { get; set; }
        private object cacheKey { get; set; }

        protected ApiCore(IHttpClientFactory factory, IMemoryCache memoryCache, object cacheKey, bool externalLink = false)
        {
            _http = factory.CreateClient("RetryHttpClient");
            _cache = memoryCache;
            baseEndpoint = BaseApi(externalLink);
            this.cacheKey = cacheKey;
        }

        private string BaseApi(bool externalLink)
        {
            if (externalLink) return "";
            else return _http.BaseAddress?.ToString().Contains("localhost") ?? true ? "http://localhost:7071/api/" : _http.BaseAddress.ToString() + "api/";
        }

        protected async Task<T?> GetAsync(string endpoint, RenderControlCore<T?>? core, object? customCacheKey = null)
        {
            cacheKey = customCacheKey ?? cacheKey;

            core?.LoadingStarted?.Invoke();

            var result = cacheKey != null ? _cache.Get<T>(cacheKey) : default;

            try
            {
                if (result == null)
                {
                    result = await _http.GetJsonFromApi<T>($"{baseEndpoint}{endpoint}");

                    if (cacheKey != null) _cache.Set(cacheKey, result, CacheSettings);
                }

                return result;
            }
            finally
            {
                core?.LoadingFinished?.Invoke(result);
            }
        }

        /// <summary>
        /// Implement Equals and GetHashCode in classes
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="customCacheKey"></param>
        protected async Task<HashSet<T>> GetListAsync(string endpoint, RenderControlCore<HashSet<T>>? core, object? customCacheKey = null)
        {
            cacheKey = customCacheKey ?? cacheKey;

            core?.LoadingStarted?.Invoke();

            var result = cacheKey != null ? _cache.Get<HashSet<T>>(cacheKey) : default;

            try
            {
                if (result == null)
                {
                    result = await _http.GetJsonFromApi<HashSet<T>>($"{baseEndpoint}{endpoint}");

                    if (cacheKey != null) _cache.Set(cacheKey, result, CacheSettings);
                }

                return result ?? [];
            }
            finally
            {
                core?.LoadingFinished?.Invoke(result);
            }
        }

        protected async Task<T?> PostAsync<I>(string endpoint, RenderControlCore<T?>? core, I? obj) where I : class
        {
            T? result = default;

            try
            {
                //TODO: create remove api
                //ArgumentNullException.ThrowIfNull(obj);

                core?.ProcessingStarted?.Invoke();

                var response = await _http.PostAsJsonAsync($"{baseEndpoint}{endpoint}", obj, new JsonSerializerOptions());

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    DataChanged?.Invoke(default);

                    return default;
                }
                else if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<T>();

                    DataChanged?.Invoke(result);

                    if (cacheKey != null) _cache.Set(cacheKey, result, CacheSettings);

                    return result;
                }
                else
                {
                    throw new NotificationException(response.ReasonPhrase);
                }
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
                //ArgumentNullException.ThrowIfNull(obj);

                core?.ProcessingStarted?.Invoke();

                var response = await _http.PutAsJsonAsync($"{baseEndpoint}{endpoint}", obj, new JsonSerializerOptions());

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    DataChanged?.Invoke(default);

                    return default;
                }
                else if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<T>();

                    DataChanged?.Invoke(result);

                    if (cacheKey != null) _cache.Set(cacheKey, result, CacheSettings);

                    return result;
                }
                else
                {
                    throw new NotificationException(response.ReasonPhrase);
                }
            }
            finally
            {
                core?.ProcessingFinished?.Invoke(result);
            }
        }
    }
}