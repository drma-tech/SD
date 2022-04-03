using SD.Shared.Helper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Core
{
    public static class ApiCore
    {
        public static async Task<T> Get<T>(this HttpClient http, string request_uri, CancellationToken cancellationToken) where T : class
        {
            var response = await http.GetAsync(request_uri, cancellationToken);

            if (!response.IsSuccessStatusCode) throw new NotificationException(response);

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
    }
}