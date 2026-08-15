using System.Text.Json.Serialization.Metadata;

namespace SD.WEB.Core.Api;

/// <summary>
///
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="factory"></param>
/// <param name="type"></param>
/// <param name="key">If data is modified by the user themselves, this key activates version control (only if the API has cache control)</param>
/// <param name="extraKeys">keys of other APIs that can be modified by this API (only if the API has cache control)</param>
/// <param name="typeInfo"></param>
public abstract class ApiCosmos<T>(IHttpClientFactory factory, ApiType type, string? key, string[] extraKeys, JsonTypeInfo<T?> typeInfo) : ApiCore(factory, key, extraKeys, type) where T : class
{
    public Action<T?>? DataChanged { get; set; }

    protected async Task<T?> GetAsync(string endpoint, bool setNewVersion, RenderControlState<T>? state, CancellationToken cancellationToken)
    {
        return await base.GetAsync<T>(endpoint, setNewVersion, state, cancellationToken);
    }

    protected async Task<IEnumerable<T>> GetListAsync(string endpoint, RenderControlState<IEnumerable<T>>? state, CancellationToken cancellationToken)
    {
        return await base.GetListAsync<T>(endpoint, state, cancellationToken);
    }

    protected async Task<T> PostAsync(string endpoint, T? obj, RenderControlState<T>? state, CancellationToken cancellationToken)
    {
        return await PostAsync(endpoint, obj, typeInfo, typeInfo, state, cancellationToken);
    }

    protected async Task<T> PostAsync<TObj>(string endpoint, TObj? obj, JsonTypeInfo<TObj?> requestTypeInfo, RenderControlState<T>? state, CancellationToken cancellationToken) where TObj : class
    {
        var result = await base.PostAsync<TObj, T>(endpoint, obj, requestTypeInfo, typeInfo, state, cancellationToken);

        DataChanged?.Invoke(result);

        return result;
    }

    protected async Task<T> PutAsync<TObj>(string endpoint, TObj? obj, JsonTypeInfo<TObj?> requestTypeInfo, RenderControlState<T>? state, CancellationToken cancellationToken) where TObj : class
    {
        var result = await base.PutAsync<TObj, T>(endpoint, obj, requestTypeInfo, typeInfo, state, cancellationToken);

        DataChanged?.Invoke(result);

        return result;
    }
}
