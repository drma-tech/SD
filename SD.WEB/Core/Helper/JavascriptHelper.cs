using Microsoft.JSInterop;
using SD.WEB.Shared;
using System.Text.Json;

namespace SD.WEB.Core.Helper
{
    public static class JsModuleLoader
    {
        private static readonly Dictionary<string, IJSObjectReference> cache = [];

        public static async Task<IJSObjectReference> Load(IJSRuntime js, string path)
        {
            if (!cache.TryGetValue(path, out var module))
            {
                module = await js.InvokeAsync<IJSObjectReference>("import", path);
                cache[path] = module;
            }

            return module;
        }
    }

    public abstract class JsModuleBase(IJSRuntime js, string path)
    {
        protected async Task InvokeVoid(string method, params object?[] args)
        {
            var module = await JsModuleLoader.Load(js, path);
            await module.InvokeVoidAsync(method, args);
        }

        protected async Task<T> Invoke<T>(string method, params object?[] args)
        {
            var module = await JsModuleLoader.Load(js, path);
            return await module.InvokeAsync<T>(method, args);
        }
    }

    public static class JsModules
    {
        public static WindowJs Window(this IJSRuntime js) => new(js);

        public static UtilsJs Utils(this IJSRuntime js) => new(js);

        public static FirebaseJs Firebase(this IJSRuntime js) => new(js);

        public static ServicesJs Services(this IJSRuntime js) => new(js);

        public static SwiperJs Swiper(this IJSRuntime js) => new(js);

        public static PaymentsJs Payments(this IJSRuntime js) => new(js);
    }

    public class WindowJs(IJSRuntime js)
    {
        public async Task HistoryBack() => await js.InvokeVoidAsync("history.back");
    }

    public class UtilsJs(IJSRuntime js) : JsModuleBase(js, "./js/utils.js")
    {
        #region STORAGE

        public Task<string?> GetLocalStorage(string key) => Invoke<string?>("storage.getLocalStorage", key);

        public async Task<TValue?> GetLocalStorage<TValue>(string key)
        {
            var value = await Invoke<string?>("storage.getLocalStorage", key);
            return value != null ? JsonSerializer.Deserialize<TValue>(value) : default;
        }

        public Task SetLocalStorage(string key, string value) => InvokeVoid("storage.setLocalStorage", key, value);

        public Task SetLocalStorage(string key, object value) => InvokeVoid("storage.setLocalStorage", key, JsonSerializer.Serialize(value));

        public Task<string?> GetSessionStorage(string key) => Invoke<string?>("storage.getSessionStorage", key);

        public async Task<TValue?> GetSessionStorage<TValue>(string key)
        {
            var value = await Invoke<string?>("storage.getSessionStorage", key);
            return value != null ? JsonSerializer.Deserialize<TValue>(value) : default;
        }

        public Task SetSessionStorage(string key, string value) => InvokeVoid("storage.setSessionStorage", key, value);

        public Task SetSessionStorage(string key, object value) => InvokeVoid("storage.setSessionStorage", key, JsonSerializer.Serialize(value));

        public Task ShowCache() => InvokeVoid("storage.showCache");

        #endregion STORAGE

        #region NOTIFICATION

        public Task<string?> PlayBeep(int frequency, int duration, string type) => Invoke<string?>("notification.playBeep", frequency, duration, type);

        public Task<string?> Vibrate(int[] pattern) => Invoke<string?>("notification.vibrate", pattern);

        #endregion NOTIFICATION

        #region INTEROP

        public Task DownloadFile(string filename, string contentType, byte[] content) => InvokeVoid("interop.downloadFile", filename, contentType, content);

        #endregion INTEROP
    }

    public class FirebaseJs(IJSRuntime js) : JsModuleBase(js, "./js/firebase.js")
    {
        public async Task SignInAsync(string providerName)
        {
            ApiCore.ResetCacheVersion();
            await InvokeVoid("authentication.signIn", providerName);
        }

        public Task SignOutAsync() => InvokeVoid("authentication.signOut");
    }

    public class ServicesJs(IJSRuntime js) : JsModuleBase(js, "./js/services.js")
    {
        public Task InitGoogleAnalytics(string version) => InvokeVoid("services.initGoogleAnalytics", version);

        public Task InitUserBack(string version) => InvokeVoid("services.initUserBack", version);

        public Task InitAdSense(string adClient, GoogleAdSense.AdUnit adSlot, string? adFormat, string containerId) => InvokeVoid("services.initAdSense", adClient, ((long)adSlot).ToString(), adFormat, containerId);
    }

    public class SwiperJs(IJSRuntime js) : JsModuleBase(js, "./js/swiper.js")
    {
        public Task InitLists(string id, int? size = null) => InvokeVoid("swiper.initLists", id, size);

        public Task InitNews(string id) => InvokeVoid("swiper.initNews", id);

        public Task InitTrailers(string id) => InvokeVoid("swiper.initTrailers", id);
    }

    public class PaymentsJs(IJSRuntime js) : JsModuleBase(js, "./js/payments.js")
    {
        public Task PaddleStart(string? token) => InvokeVoid("paddle.start", token);

        public Task PaddleOpenCheckout(string? priceId, string? email, AppLanguage locale, string? customerId) => InvokeVoid("paddle.openCheckout", priceId, email, locale.ToString(), customerId);

        public Task AppleOpenCheckout(string? productId) => InvokeVoid("apple.openCheckout", productId);

        public Task GoogleOpenCheckout(string? productId, string type) => InvokeVoid("google.openCheckout", productId, type);
    }
}