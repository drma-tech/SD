using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using SD.WEB.Modules.Subscription.Core;
using System.Globalization;
using System.Security.Claims;

namespace SD.WEB.Core;

public static class AppStateStatic
{
    public static string? Token { get; set; }
    public static bool IsAuthenticated { get; set; }
    public static bool IsPremiumUser { get; set; }
    public static ClaimsPrincipal? User { get; set; }
    public static string? UserId { get; set; }

    public static Breakpoint Breakpoint { get; set; } = Breakpoint.Xs;
    public static Action<Breakpoint>? BreakpointChanged { get; set; }
    public static Size Size { get; set; } = Size.Small;

    public static BrowserWindowSize? BrowserWindowSize { get; set; }
    public static Action<BrowserWindowSize>? BrowserWindowSizeChanged { get; set; }

    public static string? Version { get; set; }
    public static string? Search { get; set; }

    public static async Task<string> GetAppVersion(IJSRuntime js)
    {
        try
        {
            var vs = await js.Utils().GetAppVersion();

            return vs?.ReplaceLineEndings("").Trim() ?? "version-error";
        }
        catch (Exception)
        {
            return "version-error";
        }
    }

    #region Platform

    private static Platform? _platform;
    private static readonly SemaphoreSlim _platformSemaphore = new(1, 1);

    public static Platform? GetSavedPlatform()
    {
        return _platform;
    }

    public static async Task<Platform> GetPlatform(IJSRuntime js)
    {
        await _platformSemaphore.WaitAsync();
        try
        {
            if (_platform.HasValue)
            {
                return _platform.Value;
            }

            var cache = await js.Utils().GetStorage<Platform?>("platform");

            if (cache.HasValue)
            {
                _platform = cache.Value;
            }
            else
            {
                _platform = Platform.webapp;
                await js.Utils().SetStorage("platform", _platform);
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

    public static async Task<AppLanguage> GetAppLanguage(IJSRuntime js)
    {
        await _appLanguageSemaphore.WaitAsync();
        try
        {
            if (_appLanguage.HasValue)
            {
                return _appLanguage.Value;
            }

            var cache = await js.Utils().GetStorage<AppLanguage?>("app-language");

            if (cache.HasValue)
            {
                _appLanguage = cache.Value;
            }
            else
            {
                var code = await js.Window().InvokeAsync<string>("eval", "navigator.language || navigator.userLanguage");
                code = code[..2].ToLowerInvariant();

                _appLanguage = ConvertAppLanguage(code) ?? AppLanguage.en;
                await js.Utils().SetStorage("app-language", _appLanguage);
            }

            return _appLanguage.Value;
        }
        catch
        {
            return AppLanguage.en;
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

    #region DarkMode

    public static Action<bool>? DarkModeChanged { get; set; }

    private static bool? _darkMode;
    private static readonly SemaphoreSlim _darkModeSemaphore = new(1, 1);

    public static async Task<bool?> GetDarkMode(IJSRuntime js)
    {
        await _darkModeSemaphore.WaitAsync();
        try
        {
            if (_darkMode.HasValue)
            {
                return _darkMode.Value;
            }

            _darkMode = await js.Utils().GetStorage<bool?>("dark-mode");

            return _darkMode;
        }
        catch
        {
            return null;
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

    public static string? GetSavedCountry()
    {
        return _country;
    }

    public static async Task<string?> GetCountry(IpInfoApi api, IpInfoServerApi serverApi, IJSRuntime js)
    {
        await _countrySemaphore.WaitAsync();
        try
        {
            if (_country.NotEmpty())
            {
                return _country;
            }

            var cache = await js.Utils().GetStorage<string>("country");

            if (cache.NotEmpty())
            {
                _country = cache.Trim();
            }
            else
            {
                _country = (await api.GetCountry())?.Trim();

                if (_country.NotEmpty())
                    await js.Utils().SetStorage("country", _country);
                else
                    _country = await GetCountryFromApiServer(serverApi, js);
            }

            return _country;
        }
        catch
        {
            return await GetCountryFromApiServer(serverApi, js);
        }
        finally
        {
            _countrySemaphore.Release();
        }
    }

    private static async Task<string?> GetCountryFromApiServer(IpInfoServerApi serverApi, IJSRuntime js)
    {
        try
        {
            var country = (await serverApi.GetCountry())?.Trim();
            if (country.NotEmpty()) await js.Utils().SetStorage("country", country);

            return country;
        }
        catch
        {
            return null;
        }
    }

    #endregion Region Country

    #region Region

    private static Region? _region;
    private static readonly SemaphoreSlim _regionSemaphore = new(1, 1);

    public static Action<Region>? RegionChanged { get; set; }

    public static async Task<Region> GetRegion(IpInfoApi? api = null, IpInfoServerApi? serverApi = null, IJSRuntime? js = null)
    {
        await _regionSemaphore.WaitAsync();
        try
        {
            if (_region.HasValue)
            {
                return _region.Value;
            }

            var cache = js != null ? await js.Utils().GetStorage<string>("region") : null;

            if (cache.NotEmpty())
            {
                _region = ConvertRegion(cache);

                if (_region == null)
                {
                    _region = Region.US;
                    if (js != null) await js.Utils().SetStorage("region", _region);
                }
            }
            else
            {
                var code = api != null && serverApi != null && js != null ? await GetCountry(api, serverApi, js) : null;

                _region = ConvertRegion(code) ?? Region.US;
                if (js != null) await js.Utils().SetStorage("region", _region);
            }

            return _region.Value;
        }
        finally
        {
            _regionSemaphore.Release();
        }
    }

    private static Region? ConvertRegion(string? code)
    {
        if (code.Empty()) return null;

        if (Enum.TryParse<Region>(code, true, out var region) && Enum.IsDefined(region))
            return region;
        else
            return null;
    }

    public static void ChangeRegion(Region value)
    {
        _region = value;
        RegionChanged?.Invoke(value);
    }

    #endregion Region

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

            var cache = js != null ? await js.Utils().GetStorage<ContentLanguage?>("content-language") : null;

            if (cache.HasValue)
            {
                _contentLanguage = cache;
            }
            else
            {
                var culture = CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture;
                var parts = culture.Name.Split('-');
                var code = string.Join("", parts);

                _contentLanguage = ConvertContentLanguage(code) ?? ContentLanguage.enUS;
                if (js != null) await js.Utils().SetStorage("content-language", _contentLanguage);
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

    public static void ChangeContentLanguage(ContentLanguage value)
    {
        _contentLanguage = value;
    }

    #endregion ContentLanguage

    public static Action<string?>? AuthChanged { get; set; }
    public static Action<string, string>? NotificationEnabled { get; set; }
    public static Action? UserStateChanged { get; set; }
    public static Action? RegistrationSuccessful { get; set; }
    public static Action<string>? AppleVerify { get; set; }
    public static Action<string>? ShowError { get; set; }
    public static Action? ProcessingStarted { get; set; }
    public static Action? ProcessingFinished { get; set; }

    public static int TotalEnergy { get; set; } = 10;
    public static int ConsumedEnergy { get; set; } = 0;
    public static Action? EnergyConsumed { get; set; }
}