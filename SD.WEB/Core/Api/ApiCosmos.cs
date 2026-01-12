namespace SD.WEB.Core.Api;

public abstract class ApiCosmos<T>(IHttpClientFactory factory, ApiType type, string? key) : ApiCore(factory, key, type) where T : class
{
    public Action<T?>? DataChanged { get; set; }

    protected async Task<string?> GetValueAsync(string endpoint)
    {
        return await base.GetValueAsync(endpoint);
    }

    protected async Task<T?> GetAsync(string endpoint, bool setNewVersion = false)
    {
        return await base.GetAsync<T>(endpoint, setNewVersion);
    }

    protected async Task<HashSet<T>> GetListAsync(string endpoint)
    {
        return await base.GetListAsync<T>(endpoint);
    }

    protected async Task<T?> PostAsync(string endpoint, T? obj)
    {
        return await PostAsync<T>(endpoint, obj);
    }

    protected async Task<T?> PostAsync<I>(string endpoint, I? obj) where I : class
    {
        var result = await base.PostAsync<I, T>(endpoint, obj);

        DataChanged?.Invoke(result);

        return result;
    }

    protected async Task<T?> PutAsync<I>(string endpoint, I? obj) where I : class
    {
        var result = await base.PutAsync<I, T>(endpoint, obj);

        DataChanged?.Invoke(result);

        return result;
    }

    protected async Task<T?> DeleteAsync(string endpoint)
    {
        var result = await base.DeleteAsync<T>(endpoint);

        DataChanged?.Invoke(result);

        return result;
    }
}