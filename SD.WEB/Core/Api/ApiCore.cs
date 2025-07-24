using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;

namespace SD.WEB.Core.Api;

public abstract class ApiCore(IHttpClientFactory factory, string? key)
{
    protected HttpClient Http => factory.CreateClient("RetryHttpClient");
    protected static Dictionary<string, int> CacheVersion { get; set; } = [];

    public static void SetNewVersion(string? key)
    {
        if (key.NotEmpty()) CacheVersion[key!] = RandomNumberGenerator.GetInt32(1, 999999);
    }

    private Dictionary<string, string> GetVersion()
    {
        if (!CacheVersion.TryGetValue(key!, out _)) CacheVersion[key!] = RandomNumberGenerator.GetInt32(1, 999999);

        return new Dictionary<string, string> { { "v", CacheVersion[key!].ToString() } };
    }

    protected async Task<string?> GetValueAsync(string uri, CancellationToken cancellationToken = default)
    {
        if (key.NotEmpty())
            return await Http.GetValueAsync(uri.ConfigureParameters(GetVersion()), cancellationToken);
        return await Http.GetValueAsync(uri, cancellationToken);
    }

    protected async Task<T?> GetAsync<T>(string uri, bool setNewVersion = false, CancellationToken cancellationToken = default)
    {
        if (setNewVersion) SetNewVersion(key);

        if (key.NotEmpty())
            return await Http.GetJsonFromApi<T>(uri.ConfigureParameters(GetVersion()), cancellationToken);
        return await Http.GetJsonFromApi<T>(uri, cancellationToken);
    }

    protected async Task<HashSet<T>> GetListAsync<T>(string uri, CancellationToken cancellationToken = default)
    {
        if (key.NotEmpty())
        {
            var result = await Http.GetJsonFromApi<HashSet<T>>(uri.ConfigureParameters(GetVersion()), cancellationToken);
            return result ?? [];
        }
        else
        {
            var result = await Http.GetJsonFromApi<HashSet<T>>(uri, cancellationToken);
            return result ?? [];
        }
    }

    protected async Task<O?> PostAsync<I, O>(string uri, I? obj)
    {
        SetNewVersion(key);

        var response = await Http.PostAsJsonAsync(uri, obj, new JsonSerializerOptions());

        if (response.StatusCode == HttpStatusCode.NoContent) return default;

        if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<O>();

        var content = await response.Content.ReadAsStringAsync();
        throw new NotificationException(content);
    }

    protected async Task<O?> PutAsync<I, O>(string uri, I? obj)
    {
        SetNewVersion(key);

        var response = await Http.PutAsJsonAsync(uri, obj, new JsonSerializerOptions());

        if (response.StatusCode == HttpStatusCode.NoContent) return default;

        if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<O>();

        var content = await response.Content.ReadAsStringAsync();
        throw new NotificationException(content);
    }

    protected async Task<T?> DeleteAsync<T>(string uri)
    {
        SetNewVersion(key);

        var response = await Http.DeleteAsync(uri);

        if (response.StatusCode == HttpStatusCode.NoContent) return default;

        if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<T>();

        var content = await response.Content.ReadAsStringAsync();
        throw new NotificationException(content);
    }
}