using Microsoft.JSInterop;

namespace SD.WEB.Core.Helper
{
    public static class JavascriptHelper
    {
        public static async Task<string?> GetLocalStorage(this IJSRuntime js, string key)
        {
            return await js.JavascriptAsync<string?>("GetLocalStorage", key);
        }

        public static async Task<TValue?> JavascriptAsync<TValue>(this IJSRuntime js, string method, params string?[]? args)
        {
            try
            {
                return await js.InvokeAsync<TValue>(method, args);
            }
            catch (Exception)
            {
                return (TValue?)(object?)null;
            }
        }

        public static async Task SetLocalStorage(this IJSRuntime js, string key, string value)
        {
            await js.JavascriptVoidAsync("SetLocalStorage", key, value);
        }

        public static async Task JavascriptVoidAsync(this IJSRuntime js, string method, params object?[]? args)
        {
            await js.InvokeVoidAsync(method, args);
        }
    }
}