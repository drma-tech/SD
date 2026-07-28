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

    protected async Task<T?> GetAsync(string endpoint, bool setNewVersion, ComponentActions<T>? actions, CancellationToken cancellationToken)
    {
        return await base.GetAsync<T>(endpoint, setNewVersion, actions, cancellationToken);
    }

    protected async Task<HashSet<T>> GetListAsync(string endpoint, ComponentActions<HashSet<T>>? actions, CancellationToken cancellationToken)
    {
        return await base.GetListAsync<T>(endpoint, actions, cancellationToken);
    }

    protected async Task<T> PostAsync(string endpoint, T? obj, CancellationToken cancellationToken)
    {
        return await PostAsync(endpoint, obj, typeInfo, typeInfo, cancellationToken);
    }

    protected async Task<T> PostAsync<I>(string endpoint, I? obj, JsonTypeInfo<I?> requestTypeInfo, CancellationToken cancellationToken) where I : class
    {
        var result = await base.PostAsync<I, T>(endpoint, obj, requestTypeInfo, typeInfo, cancellationToken);

        DataChanged?.Invoke(result);

        return result;
    }

    protected async Task<T> PutAsync<I>(string endpoint, I? obj, JsonTypeInfo<I?> requestTypeInfo, CancellationToken cancellationToken) where I : class
    {
        var result = await base.PutAsync<I, T>(endpoint, obj, requestTypeInfo, typeInfo, cancellationToken);

        DataChanged?.Invoke(result);

        return result;
    }
}
