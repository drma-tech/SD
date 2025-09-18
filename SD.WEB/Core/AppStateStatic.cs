using Microsoft.JSInterop;
using MudBlazor;
using SD.Shared.Models.Auth;
using SD.WEB.Modules.Subscription.Core;
using System.Globalization;
using System.Security.Claims;

namespace SD.WEB.Core;

public static class AppStateStatic
{
    public static bool IsAuthenticated { get; set; }
    public static ClaimsPrincipal? User { get; set; }
    public static string? UserId { get; set; }

    public static Breakpoint Breakpoint { get; set; }
    public static Action<Breakpoint>? BreakpointChanged { get; set; }

    public static string? Version { get; set; }

    #region Platform

    private static Platform? _platform;
    private static readonly SemaphoreSlim _platformSemaphore = new(1, 1);

    public static async Task<Platform> GetPlatform(IJSRuntime? js = null)
    {
        await _platformSemaphore.WaitAsync();
        try
        {
            if (_platform.HasValue)
            {
                return _platform.Value;
            }

            var cache = js != null ? await js.InvokeAsync<string>("GetLocalStorage", "platform") : null;

            if (cache.Empty() && js != null) //shouldn't happen (because it's called in index.html)
            {
                await js.InvokeVoidAsync("TryDetectPlatform");
                cache = await js.InvokeAsync<string>("GetLocalStorage", "platform");
            }

            if (cache.NotEmpty())
            {
                if (Enum.TryParse<Platform>(cache, true, out var platform) && Enum.IsDefined(platform))
                {
                    _platform = platform;
                }
                else
                {
                    _platform = Platform.webapp;
                    if (js != null) await js.InvokeVoidAsync("SetLocalStorage", "platform", _platform.ToString());
                }
            }
            else
            {
                _platform = Platform.webapp;
            }

            return _platform.Value;
        }
        finally
        {
            _platformSemaphore.Release();
        }
    }

    #endregion Platform

    #region AppLanguage

    private static AppLanguage? _appLanguage;
    private static readonly SemaphoreSlim _appLanguageSemaphore = new(1, 1);

    public static async Task<AppLanguage> GetAppLanguage(IJSRuntime? js = null)
    {
        await _appLanguageSemaphore.WaitAsync();
        try
        {
            if (_appLanguage.HasValue)
            {
                return _appLanguage.Value;
            }

            var cache = js != null ? await js.InvokeAsync<string>("GetLocalStorage", "app-language") : null;

            if (cache.NotEmpty())
            {
                _appLanguage = ConvertAppLanguage(cache);

                if (_appLanguage == null)
                {
                    _appLanguage = AppLanguage.en;
                    if (js != null) await js.InvokeVoidAsync("SetLocalStorage", "app-language", _appLanguage.ToString());
                }
            }
            else
            {
                var culture = CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture;
                var code = culture.TwoLetterISOLanguageName?.ToLowerInvariant();

                _appLanguage = ConvertAppLanguage(code) ?? AppLanguage.en;
                if (js != null) await js.InvokeVoidAsync("SetLocalStorage", "app-language", _appLanguage.ToString());
            }

            return _appLanguage.Value;
        }
        finally
        {
            _appLanguageSemaphore.Release();
        }
    }

    private static AppLanguage? ConvertAppLanguage(string? code)
    {
        if (code.Empty()) return null;

        if (Enum.TryParse<AppLanguage>(code, true, out var language) && Enum.IsDefined(language))
            return language;
        else
            return null;
    }

    #endregion AppLanguage

    #region ContentLanguage

    private static ContentLanguage? _contentLanguage;
    private static readonly SemaphoreSlim _contentLanguageSemaphore = new(1, 1);

    public static async Task<ContentLanguage> GetContentLanguage(IJSRuntime? js = null)
    {
        await _contentLanguageSemaphore.WaitAsync();
        try
        {
            if (_contentLanguage.HasValue)
            {
                return _contentLanguage.Value;
            }

            var cache = js != null ? await js.InvokeAsync<string>("GetLocalStorage", "content-language") : null;

            if (cache.NotEmpty())
            {
                _contentLanguage = ConvertContentLanguage(cache);

                if (_contentLanguage == null)
                {
                    _contentLanguage = ContentLanguage.enUS;
                    if (js != null) await js.InvokeVoidAsync("SetLocalStorage", "content-language", _contentLanguage.ToString());
                }
            }
            else
            {
                var culture = CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture;
                var parts = culture.Name.Split('-');
                var code = string.Join("", parts);

                _contentLanguage = ConvertContentLanguage(code) ?? ContentLanguage.enUS;
                if (js != null) await js.InvokeVoidAsync("SetLocalStorage", "content-language", _contentLanguage.ToString());
            }

            return _contentLanguage.Value;
        }
        finally
        {
            _contentLanguageSemaphore.Release();
        }
    }

    private static ContentLanguage? ConvertContentLanguage(string? code)
    {
        if (code.Empty()) return null;

        if (Enum.TryParse<ContentLanguage>(code, true, out var language) && Enum.IsDefined(language))
            return language;
        else
            return null;
    }

    #endregion ContentLanguage

    #region Region

    [Custom(Name = "Region", ResourceType = typeof(GlobalTranslations))]
    public static Region Region { get; set; } = Region.US;

    public static Action<Region>? RegionChanged { get; set; }

    public static Region GetValidRegion(string? regionName)
    {
        if (!Enum.TryParse<Region>(regionName, out var region) || !Enum.IsDefined(region))
        {
            regionName = nameof(Region.US);

            _ = Enum.TryParse(regionName, out region);
        }

        return region;
    }

    public static void ChangeRegion(Region value)
    {
        Region = value;
        RegionChanged?.Invoke(value);
    }

    #endregion Region

    #region DarkMode

    public static Action<bool>? DarkModeChanged { get; set; }

    private static bool? _darkMode;
    private static readonly SemaphoreSlim _darkModeSemaphore = new(1, 1);

    public static async Task<bool?> GetDarkMode(IJSRuntime? js = null)
    {
        await _darkModeSemaphore.WaitAsync();
        try
        {
            if (_darkMode.HasValue)
            {
                return _darkMode.Value;
            }

            var cache = js != null ? await js.InvokeAsync<string>("GetLocalStorage", "dark-mode") : null;

            if (cache.NotEmpty())
            {
                if (bool.TryParse(cache, out var value))
                {
                    _darkMode = value;
                }
                else
                {
                    _darkMode = false;
                    if (js != null) await js.InvokeVoidAsync("SetLocalStorage", "dark-mode", _darkMode.ToString()?.ToLower());
                }
            }

            return _darkMode;
        }
        finally
        {
            _darkModeSemaphore.Release();
        }
    }

    public static void ChangeDarkMode(bool darkMode)
    {
        _darkMode = darkMode;
        DarkModeChanged?.Invoke(darkMode);
    }

    #endregion DarkMode

    #region Region Country

    private static string? _country;
    private static readonly SemaphoreSlim _countrySemaphore = new(1, 1);

    public static async Task<string> GetCountry(IpInfoApi api, IJSRuntime? js = null)
    {
        await _countrySemaphore.WaitAsync();
        try
        {
            if (_country.NotEmpty())
            {
                return _country;
            }

            var cache = js != null ? await js.InvokeAsync<string>("GetLocalStorage", "country") : null;

            if (cache.NotEmpty())
            {
                _country = cache.Trim();
            }
            else
            {
                _country = (await api.GetCountry())?.Trim();
                if (js != null) await js.InvokeVoidAsync("SetLocalStorage", "country", _country);
            }

            _country ??= "US";

            return _country;
        }
        finally
        {
            _countrySemaphore.Release();
        }
    }

    #endregion Region Country

    public static Action<TempAuthPaddle>? RegistrationSuccessful { get; set; }
    public static Action<string>? ShowError { get; set; }
    public static Action? ProcessingStarted { get; set; }
    public static Action? ProcessingFinished { get; set; }
}