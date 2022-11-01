﻿using Blazored.SessionStorage;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SD.WEB.Core
{
    public static class ApiCore
    {
        private static async Task<T?> ReturnResponse<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T?>();
            }
            else
            {
                throw new NotificationException(response);
            }
        }

        public static JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions();
        }

        public static async Task<T?> Get<T>(this HttpClient http, string requestUri, bool externalLink, ISyncSessionStorageService? storage = null, bool forceUpdate = false) where T : class
        {
            if (storage == null)
            {
                var response = await http.GetAsync(http.BaseApi(externalLink) + requestUri);

                return await response.ReturnResponse<T>();
            }
            else
            {
                if (forceUpdate || !storage.ContainKey(requestUri))
                {
                    var response = await http.GetAsync(http.BaseApi(externalLink) + requestUri);

                    storage.SetItem(requestUri, await response.ReturnResponse<T>());
                }

                return storage.GetItem<T>(requestUri);
            }
        }

        public static async Task<T> GetNew<T>(this HttpClient http, ISyncSessionStorageService storage, string request_uri) where T : class
        {
            if (!storage.ContainKey(request_uri))
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, request_uri);

                request.Headers.Add("authorization", "Bearer <<access_token>>");
                request.Headers.TryAddWithoutValidation("content-type", "application/json;charset=utf-8");

                var response = await http.SendAsync(request);

                if (!response.IsSuccessStatusCode) throw new NotificationException(response);

                storage.SetItem(request_uri, await response.Content.ReadFromJsonAsync<T>());
            }

            return storage.GetItem<T>(request_uri);
        }

        public static async Task<List<T>> GetList<T>(this HttpClient http, string requestUri, bool externalLink, ISyncSessionStorageService? storage = null, bool forceUpdate = false) where T : class
        {
            if (storage == null)
            {
                var response = await http.GetAsync(http.BaseApi(externalLink) + requestUri);

                return await response.ReturnResponse<List<T>>() ?? new();
            }
            else
            {
                if (forceUpdate || !storage.ContainKey(requestUri))
                {
                    var response = await http.GetAsync(http.BaseApi(externalLink) + requestUri);

                    storage.SetItem(requestUri, await response.ReturnResponse<List<T>>());
                }

                return storage.GetItem<List<T>>(requestUri);
            }
        }

        public static async Task<HttpResponseMessage> Post<T>(this HttpClient http, string requestUri, bool externalLink, T obj, ISyncSessionStorageService? storage = null, string? urlGet = null) where T : class
        {
            var response = await http.PostAsJsonAsync(http.BaseApi(externalLink) + requestUri, obj, GetOptions());

            if (storage != null && !string.IsNullOrWhiteSpace(urlGet) && response.IsSuccessStatusCode)
            {
                storage.SetItem(urlGet, await response.ReturnResponse<T>());
            }

            return response;
        }

        public static async Task<HttpResponseMessage> PostNew<T>(this HttpClient http, string requestUri, bool externalLink, T obj, ISyncSessionStorageService? storage = null, string? urlGet = null) where T : class
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            request.Headers.Add("authorization", "Bearer <<access_token>>");
            request.Headers.TryAddWithoutValidation("content-type", "application/json;charset=utf-8");
            request.Content = new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

            var response = await http.SendAsync(request);
            //var response = await http.PostAsJsonAsync(http.BaseApi(externalLink) + requestUri, obj, GetOptions());

            if (storage != null && !string.IsNullOrWhiteSpace(urlGet) && response.IsSuccessStatusCode)
            {
                storage.SetItem(urlGet, await response.ReturnResponse<T>());
            }

            return response;
        }

        public static async Task<HttpResponseMessage> Put<T>(this HttpClient http, string requestUri, bool externalLink, T? obj, ISyncSessionStorageService? storage = null, string? urlGet = null) where T : class
        {
            var response = await http.PutAsJsonAsync(http.BaseApi(externalLink) + requestUri, obj, GetOptions());

            if (storage != null && !string.IsNullOrWhiteSpace(urlGet) && response.IsSuccessStatusCode)
            {
                storage.SetItem(urlGet, await response.ReturnResponse<T>());
            }

            return response;
        }

        public static async Task<HttpResponseMessage> Delete(this HttpClient http, string requestUri, bool externalLink)
        {
            return await http.DeleteAsync(http.BaseApi(externalLink) + requestUri);
        }
    }
}