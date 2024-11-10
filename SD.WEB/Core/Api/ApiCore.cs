using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SD.WEB.Core.Api
{
    public abstract class ApiCore(IHttpClientFactory factory)
    {
        protected HttpClient _http => factory.CreateClient("RetryHttpClient");

        protected async Task<T?> GetAsync<T>(string uri)
        {
            return await _http.GetJsonFromApi<T>(uri);
        }

        protected async Task<string?> GetValueAsync(string uri)
        {
            return await _http.GetValueAsync(uri);
        }

        protected async Task<HashSet<T>> GetListAsync<T>(string uri)
        {
            var result = await _http.GetJsonFromApi<HashSet<T>>(uri);
            return result ?? [];
        }

        protected async Task<T?> GetByRequest<T>(string uri)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);

            //request.Headers.Add("authorization", "Bearer <<access_token>>");
            request.Headers.TryAddWithoutValidation("content-type", "application/json;charset=utf-8");

            return await _http.GetJsonFromApi<T>(request);
        }

        protected async Task<O?> PostAsync<I, O>(string uri, I? obj)
        {
            var response = await _http.PostAsJsonAsync(uri, obj, new JsonSerializerOptions());

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }
            else if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<O>();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new NotificationException(content);
            }
        }

        protected async Task<O?> PutAsync<I, O>(string uri, I? obj)
        {
            var response = await _http.PutAsJsonAsync(uri, obj, new JsonSerializerOptions());

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }
            else if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<O>();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new NotificationException(content);
            }
        }

        protected async Task<T?> DeleteAsync<T>(string uri)
        {
            var response = await _http.DeleteAsync(uri);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }
            else if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new NotificationException(content);
            }
        }
    }
}