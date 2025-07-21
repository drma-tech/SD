using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace SD.Shared.Core.Helper;

public static class ApiHelper
{
    public static async Task<string?> GetValueAsync(this HttpClient httpClient, string uri, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(uri, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NoContent) return null;

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new NotificationException(content);
    }

    public static async Task<T?> GetJsonFromApi<T>(this HttpClient httpClient, string uri, CancellationToken cancellationToken, ILogger? logger = null)
    {
        logger?.LogWarning($"[ApiHelper] before GetAsync: Timeout={httpClient.Timeout.TotalSeconds}s, TokenCanBeCanceled={cancellationToken.CanBeCanceled}, IsCancellationRequested={cancellationToken.IsCancellationRequested}");
        var response = await httpClient.GetAsync(uri, cancellationToken);
        logger?.LogWarning($"[ApiHelper] after GetAsync: IsCancellationRequested={cancellationToken.IsCancellationRequested}, ResponseSuccess={response.IsSuccessStatusCode}");

        if (response.IsSuccessStatusCode)
        {
            try
            {
                if (response.StatusCode == HttpStatusCode.NoContent) return default;

                return await response.Content.ReadFromJsonAsync<T>(new JsonSerializerOptions(), cancellationToken);
            }
            catch (NotSupportedException ex) // When content type is not valid
            {
                throw new InvalidDataException("The content type is not supported", ex.InnerException ?? ex);
            }
            catch (JsonException ex) // Invalid JSON
            {
                throw new InvalidDataException("invalid json", ex.InnerException ?? ex);
            }
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new NotificationException(content);
        }
    }
}