﻿using SD.WEB.Shared;

namespace SD.WEB.Core.Api;

public abstract class ApiCosmos<T>(IHttpClientFactory factory, ApiType type, string? key) : ApiCore(factory, key, type) where T : class
{
    public Action<T?>? DataChanged { get; set; }

    protected async Task<string?> GetValueAsync(string endpoint, RenderControlCore<string?>? core)
    {
        core?.LoadingStarted?.Invoke();

        string? result = null;

        try
        {
            result = await GetValueAsync(endpoint);
            return result;
        }
        catch (Exception ex)
        {
            core?.ShowError(ex.Message);
            return null;
        }
        finally
        {
            core?.LoadingFinished?.Invoke(result);
        }
    }

    protected async Task<T?> GetAsync(string endpoint, RenderControlCore<T?>? core, bool setNewVersion = false)
    {
        core?.LoadingStarted?.Invoke();

        T? obj = null;

        try
        {
            obj = await GetAsync<T>(endpoint, setNewVersion);
            return obj;
        }
        catch (Exception ex)
        {
            core?.ShowError(ex.Message);
            return null;
        }
        finally
        {
            core?.LoadingFinished?.Invoke(obj);
        }
    }

    protected async Task<HashSet<T>> GetListAsync(string endpoint, RenderControlCore<HashSet<T>>? core)
    {
        core?.LoadingStarted?.Invoke();

        HashSet<T> list = [];

        try
        {
            list = await GetListAsync<T>(endpoint);
            return list;
        }
        catch (Exception ex)
        {
            core?.ShowError(ex.Message);
            return [];
        }
        finally
        {
            core?.LoadingFinished?.Invoke(list);
        }
    }

    protected async Task<T?> PostAsync(string endpoint, RenderControlCore<T?>? core, T? obj)
    {
        return await PostAsync<T>(endpoint, core, obj);
    }

    protected async Task<T?> PostAsync<I>(string endpoint, RenderControlCore<T?>? core, I? obj) where I : class
    {
        T? result = null;

        try
        {
            core?.ProcessingStarted?.Invoke();

            result = await PostAsync<I, T>(endpoint, obj);

            DataChanged?.Invoke(result);

            return result;
        }
        finally
        {
            core?.ProcessingFinished?.Invoke(result);
        }
    }

    protected async Task<T?> PutAsync<I>(string endpoint, RenderControlCore<T?>? core, I? obj) where I : class
    {
        T? result = null;

        try
        {
            core?.ProcessingStarted?.Invoke();

            result = await PutAsync<I, T>(endpoint, obj);

            DataChanged?.Invoke(result);

            return result;
        }
        finally
        {
            core?.ProcessingFinished?.Invoke(result);
        }
    }

    protected async Task<T?> DeleteAsync(string endpoint, RenderControlCore<T?>? core)
    {
        T? result = null;

        try
        {
            core?.ProcessingStarted?.Invoke();

            result = await DeleteAsync<T>(endpoint);

            DataChanged?.Invoke(result);

            return result;
        }
        finally
        {
            core?.ProcessingFinished?.Invoke(result);
        }
    }
}