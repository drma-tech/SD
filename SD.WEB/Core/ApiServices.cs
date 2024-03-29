﻿using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SD.WEB.Core
{
    public class CacheSettings : MemoryCacheEntryOptions
    {
        public CacheSettings(TimeSpan? SlidingExpiration = null, TimeSpan? AbsoluteExpirationRelativeToNow = null)
        {
            this.SlidingExpiration = SlidingExpiration ?? TimeSpan.FromMinutes(5);
            this.AbsoluteExpirationRelativeToNow = AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(10);
        }
    }

    public abstract class ApiServices
    {
        protected HttpClient Http { get; set; }
        protected IMemoryCache MemoryCache { get; set; }

        protected ApiServices(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            Http = httpClientFactory.CreateClient("RetryHttpClient");
            MemoryCache = memoryCache;
        }

        private string BaseApi(bool isExternalLink)
        {
            if (isExternalLink) return "";
            else return Http.BaseAddress?.ToString().Contains("localhost") ?? true ? "http://localhost:7071/api/" : Http.BaseAddress.ToString() + "api/";
        }

        protected void CleanCache(string? urlGet = null)
        {
            if (string.IsNullOrEmpty(urlGet)) return;

            MemoryCache.Remove(urlGet);
        }

        protected async Task<string?> GetValueAsync(string requestUri, bool isExternalLink, CacheSettings? cacheSettings = null)
        {
            if (MemoryCache == null)
            {
                return await Http.GetValueAsync(BaseApi(isExternalLink) + requestUri);
            }
            else
            {
                cacheSettings ??= new CacheSettings();

                var result = MemoryCache.Get<string>(requestUri);

                if (result == null)
                {
                    result = await Http.GetValueAsync(BaseApi(isExternalLink) + requestUri);

                    MemoryCache.Set(requestUri, result, cacheSettings);
                }

                return result;
            }
        }

        /// <summary>
        /// Return a HashSet<T>
        /// Note: Implement Equals and GetHashCode in classes
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="isExternalLink"></param>
        /// <param name="cacheSettings"></param>
        /// <returns></returns>
        /// <exception cref="NotificationException"></exception>
        protected async Task<HashSet<T>> GetListAsync<T>(string requestUri, bool isExternalLink, CacheSettings? cacheSettings = null) where T : class
        {
            if (MemoryCache == null)
            {
                return await Http.GetJsonFromApi<HashSet<T>>(BaseApi(isExternalLink) + requestUri) ?? [];
            }
            else
            {
                cacheSettings ??= new CacheSettings();

                var result = MemoryCache.Get<HashSet<T>>(requestUri);

                if (result == null)
                {
                    result = await Http.GetJsonFromApi<HashSet<T>>(BaseApi(isExternalLink) + requestUri) ?? [];

                    MemoryCache.Set(requestUri, result, cacheSettings);
                }

                return result;
            }
        }

        protected async Task<T?> GetAsync<T>(string requestUri, bool isExternalLink, CacheSettings? cacheSettings = null) where T : class
        {
            if (MemoryCache == null)
            {
                return await Http.GetJsonFromApi<T>(BaseApi(isExternalLink) + requestUri);
            }
            else
            {
                cacheSettings ??= new CacheSettings();

                var result = MemoryCache.Get<T>(requestUri);

                if (result == null)
                {
                    result = await Http.GetJsonFromApi<T>(BaseApi(isExternalLink) + requestUri);

                    MemoryCache.Set(requestUri, result, cacheSettings);
                }

                return result;
            }
        }

        protected async Task<T?> GetByRequest<T>(string requestUri, CacheSettings? cacheSettings = null) where T : class
        {
            if (MemoryCache == null)
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

                //request.Headers.Add("authorization", "Bearer <<access_token>>");
                request.Headers.TryAddWithoutValidation("content-type", "application/json;charset=utf-8");

                return await Http.GetJsonFromApi<T>(request);
            }
            else
            {
                cacheSettings ??= new CacheSettings();

                var result = MemoryCache.Get<T>(requestUri);

                if (result == null)
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

                    //request.Headers.Add("authorization", "Bearer <<access_token>>");
                    request.Headers.TryAddWithoutValidation("content-type", "application/json;charset=utf-8");

                    result = await Http.GetJsonFromApi<T>(request);

                    MemoryCache.Set(requestUri, result, cacheSettings);
                }

                return result;
            }
        }

        protected async Task<T?> PostAsync<T>(string requestUri, bool isExternalLink, T? obj, string? urlGet = null, CacheSettings? cacheSettings = null) where T : class
        {
            return await PostAsync<T, T>(requestUri, isExternalLink, obj, urlGet, cacheSettings);
        }

        protected async Task<O?> PostAsync<I, O>(string requestUri, bool isExternalLink, I? obj, string? urlGet = null, CacheSettings? cacheSettings = null) where I : class where O : class
        {
            var response = await Http.PostAsJsonAsync(BaseApi(isExternalLink) + requestUri, obj, new JsonSerializerOptions());

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }
            else if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<O>();

                if (MemoryCache != null && !string.IsNullOrEmpty(urlGet))
                {
                    cacheSettings ??= new CacheSettings();

                    MemoryCache.Set(urlGet, result, cacheSettings);
                }

                return result;
            }
            else
            {
                throw new NotificationException(response.ReasonPhrase);
            }
        }

        protected async Task<T?> PutAsync<T>(string requestUri, bool isExternalLink, T? obj, string? urlGet = null, CacheSettings? cacheSettings = null) where T : class
        {
            var response = await Http.PutAsJsonAsync(BaseApi(isExternalLink) + requestUri, obj, new JsonSerializerOptions());

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }
            else if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>();

                if (MemoryCache != null && !string.IsNullOrEmpty(urlGet))
                {
                    cacheSettings ??= new CacheSettings();

                    MemoryCache.Set(urlGet, result, cacheSettings);
                }

                return result;
            }
            else
            {
                throw new NotificationException(response.ReasonPhrase);
            }
        }

        protected async Task<bool> DeleteAsync(string requestUri, bool isExternalLink, string? urlGet = null)
        {
            var response = await Http.DeleteAsync(BaseApi(isExternalLink) + requestUri);

            if (response.IsSuccessStatusCode)
            {
                if (MemoryCache != null && !string.IsNullOrEmpty(urlGet))
                {
                    MemoryCache.Remove(urlGet);
                }

                return true;
            }
            else
            {
                throw new NotificationException(response.ReasonPhrase);
            }
        }
    }
}