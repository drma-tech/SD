using System.Text.Json.Serialization.Metadata;

namespace SD.WEB.Api.Core;

public enum ApiType
{
    Anonymous,
    Authenticated,
}

public abstract class ApiCosmos<T>(IHttpClientFactory factory, ApiType type, string? key, string[] extraKeys, JsonTypeInfo<T?> typeInfo) : ApiCore(key, extraKeys) where T : class
{
    protected HttpClient AnonymousHttp => factory.CreateClient("Anonymous");
    protected HttpClient AuthenticatedHttp => factory.CreateClient("Authenticated");
    public Action<T?>? DataChanged { get; set; }

    private HttpClient GetHttp() => type switch
    {
        ApiType.Anonymous => AnonymousHttp,
        ApiType.Authenticated => AuthenticatedHttp,
        _ => throw new NotSupportedException(),
    };

    protected async Task<string?> GetStringAsync(string endpoint, CancellationToken cancellationToken)
    {
        return await GetStringAsync(GetHttp(), endpoint, cancellationToken);
    }

    protected async Task<bool> GetBoolAsync(string endpoint, CancellationToken cancellationToken)
    {
        return await GetBoolAsync(GetHttp(), endpoint, cancellationToken);
    }

    protected async Task<T?> GetAsync(string endpoint, bool setNewVersion, RenderControlState<T>? state, CancellationToken cancellationToken)
    {
        return await GetAsync(GetHttp(), endpoint, setNewVersion, state, cancellationToken);
    }

    protected async Task<IEnumerable<T>> GetListAsync(string endpoint, RenderControlState<IEnumerable<T>>? state, CancellationToken cancellationToken)
    {
        return await GetListAsync(GetHttp(), endpoint, state, cancellationToken);
    }

    protected async Task<T> PostAsync(string endpoint, T? obj, RenderControlState<T>? state, CancellationToken cancellationToken)
    {
        var result = await PostAsync(GetHttp(), endpoint, obj, typeInfo, typeInfo, state, cancellationToken);

        DataChanged?.Invoke(result);

        return result;
    }

    protected async Task<T> PostAsync<TObj>(string endpoint, TObj? obj, JsonTypeInfo<TObj?> requestTypeInfo, RenderControlState<T>? state, CancellationToken cancellationToken) where TObj : class
    {
        var result = await PostAsync(GetHttp(), endpoint, obj, requestTypeInfo, typeInfo, state, cancellationToken);

        DataChanged?.Invoke(result);

        return result;
    }

    protected async Task<T> PutAsync<TObj>(string endpoint, TObj? obj, JsonTypeInfo<TObj?> requestTypeInfo, RenderControlState<T>? state, CancellationToken cancellationToken) where TObj : class
    {
        var result = await PutAsync(GetHttp(), endpoint, obj, requestTypeInfo, typeInfo, state, cancellationToken);

        DataChanged?.Invoke(result);

        return result;
    }

    protected async Task DeleteAsync(string endpoint, CancellationToken cancellationToken)
    {
        await DeleteAsync(GetHttp(), endpoint, cancellationToken);
    }
}