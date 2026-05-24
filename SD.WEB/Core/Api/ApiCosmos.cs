using System.Text.Json.Serialization.Metadata;

namespace SD.WEB.Core.Api;

public abstract class ApiCosmos<T>(IHttpClientFactory factory, ApiType type, string? key, JsonTypeInfo<T?> typeInfo) : ApiCore(factory, key, type) where T : class
{
    public Action<T?>? DataChanged { get; set; }

    protected async Task<T?> GetAsync(string endpoint, bool setNewVersion, CancellationToken cancellationToken)
    {
        return await base.GetAsync<T>(endpoint, setNewVersion, cancellationToken);
    }

    protected async Task<HashSet<T>> GetListAsync(string endpoint, CancellationToken cancellationToken)
    {
        return await base.GetListAsync<T>(endpoint, cancellationToken);
    }

    protected async Task<T?> PostAsync(string endpoint, T? obj, CancellationToken cancellationToken)
    {
        return await PostAsync(endpoint, obj, typeInfo, typeInfo, cancellationToken);
    }

    protected async Task<T?> PostAsync<I>(string endpoint, I? obj, JsonTypeInfo<I?> requestTypeInfo, CancellationToken cancellationToken) where I : class
    {
        var result = await base.PostAsync<I, T>(endpoint, obj, requestTypeInfo, typeInfo, cancellationToken);

        DataChanged?.Invoke(result);

        return result;
    }

    protected async Task<T?> PutAsync<I>(string endpoint, I? obj, JsonTypeInfo<I?> requestTypeInfo, CancellationToken cancellationToken) where I : class
    {
        var result = await base.PutAsync<I, T>(endpoint, obj, requestTypeInfo, typeInfo, cancellationToken);

        DataChanged?.Invoke(result);

        return result;
    }

    protected async Task<T?> DeleteAsync(string endpoint, CancellationToken cancellationToken)
    {
        var result = await base.DeleteAsync<T>(endpoint, typeInfo, cancellationToken);

        DataChanged?.Invoke(result);

        return result;
    }
}