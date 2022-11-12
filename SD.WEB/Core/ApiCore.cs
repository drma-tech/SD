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

        public static async Task<T?> Get<T>(this HttpClient http, string requestUri, bool externalLink) where T : class
        {
            var response = await http.GetAsync(http.BaseApi(externalLink) + requestUri);

            return await response.ReturnResponse<T>();
        }

        public static async Task<T?> GetNew<T>(this HttpClient http, string request_uri) where T : class
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, request_uri);

            request.Headers.Add("authorization", "Bearer <<access_token>>");
            request.Headers.TryAddWithoutValidation("content-type", "application/json;charset=utf-8");

            var response = await http.SendAsync(request);

            if (!response.IsSuccessStatusCode) throw new NotificationException(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public static async Task<List<T>> GetList<T>(this HttpClient http, string requestUri, bool externalLink) where T : class
        {
            var response = await http.GetAsync(http.BaseApi(externalLink) + requestUri);

            return await response.ReturnResponse<List<T>>() ?? new();
        }

        public static async Task<HttpResponseMessage> Post<T>(this HttpClient http, string requestUri, bool externalLink, T obj, string? urlGet = null) where T : class
        {
            var response = await http.PostAsJsonAsync(http.BaseApi(externalLink) + requestUri, obj, GetOptions());

            return response;
        }

        public static async Task<HttpResponseMessage> PostNew<T>(this HttpClient http, string requestUri, bool externalLink, T obj, string? urlGet = null) where T : class
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            request.Headers.Add("authorization", "Bearer <<access_token>>");
            request.Headers.TryAddWithoutValidation("content-type", "application/json;charset=utf-8");
            request.Content = new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

            var response = await http.SendAsync(request);
            //var response = await http.PostAsJsonAsync(http.BaseApi(externalLink) + requestUri, obj, GetOptions());

            return response;
        }

        public static async Task<HttpResponseMessage> Put<T>(this HttpClient http, string requestUri, bool externalLink, T? obj, string? urlGet = null) where T : class
        {
            var response = await http.PutAsJsonAsync(http.BaseApi(externalLink) + requestUri, obj, GetOptions());

            return response;
        }

        public static async Task<HttpResponseMessage> Delete(this HttpClient http, string requestUri, bool externalLink)
        {
            return await http.DeleteAsync(http.BaseApi(externalLink) + requestUri);
        }
    }
}