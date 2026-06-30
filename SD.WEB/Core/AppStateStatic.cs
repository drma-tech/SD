using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SD.Shared.Models.Auth;
using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Subscription.Core;
using System.Security.Claims;

namespace SD.WEB.Core;

public static class AppStateStatic
{
    public static string? SupabaseToken { get; set; }
    public static bool IsAuthenticated { get; set; }
    public static bool IsPremiumUser { get; set; }
    public static AccountProduct ActiveProduct { get; set; }
    public static ClaimsPrincipal? User { get; set; }
    public static AuthPrincipal? Principal { get; set; }
    public static string? UserId { get; set; }
    public static DateTimeOffset? LastAccess { get; set; } //control login, so we don't call api too often

    public static Size Size { get; set; } = Size.Small;
    public static Breakpoint Breakpoint { get; set; } = Breakpoint.Xs;
    public static ActionDispatcher<Breakpoint> BreakpointChanged { get; } = new();

    public static string? Version { get; set; }
    public static string? BrowserName { get; set; }
    public static string? BrowserVersion { get; set; }
    public static string? OperatingSystem { get; set; }

    private static string? LastSnackbarMessage { get; set; }
    private static DateTime LastSnackbarAt { get; set; } = DateTime.MinValue;
    private static readonly TimeSpan SnackbarDelay = TimeSpan.FromSeconds(10);

    public static bool IsLocalhost(this NavigationManager navigation)
    {
        return navigation.BaseUri.Contains("localhost") || navigation.BaseUri.Contains("develop");
    }

    public static bool IsPrerendering(this NavigationManager navigation)
    {
        return navigation.BaseUri.Contains("127.0.0.1");
    }

    public static bool CanShowSnackbar(this string message)
    {
        var now = DateTime.UtcNow;

        if (LastSnackbarMessage == message && now - LastSnackbarAt < SnackbarDelay)
        {
            return false;
        }

        LastSnackbarMessage = message;
        LastSnackbarAt = now;

        return true;
    }

    #region Search

    public static int Index { get; set; } = 0;
    public static string? Query { get; set; }
    public static HashSet<TmdbResultKeyword> Keywords { get; set; } = [];
    public static MediaType? Type;
    public static MovieGenre? MovieGenre;
    public static TvGenre? TvGenre;
    public static string SortBy = "popularity.desc";

    #endregion Search

    public static async Task<string> GetAppVersion(IJSRuntime js, CancellationToken cancellationToken)
    {
        try
        {
            return await js.InvokeAsync<string>("eval", cancellationToken, "window.appVersion");
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

    public static async Task<Platform> GetPlatform(IJSRuntime js, CancellationToken cancellationToken)
    {
        await _platformSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_platform.HasValue)
            {
                return _platform.Value;
            }

            var cache = await js.Utils().GetStorage("platform", JavascriptContext.Default.NullablePlatform, cancellationToken);

            if (cache.HasValue)
            {
                _platform = cache.Value;
            }
            else
            {
                _platform = Platform.webapp;
                await js.Utils().SetStorage("platform", _platform, JavascriptContext.Default.NullablePlatform, cancellationToken);
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

    public static string[] SupportedLanguages => ["en", "pt", "es", "fr", "it", "de", "zh"];
    public static string[] SitemapTranslations => ["en", "pt", "es"];

    private static AppLanguage? _appLanguage;
    private static readonly SemaphoreSlim _appLanguageSemaphore = new(1, 1);

    public static bool IsValidLanguage(this string? lang)
    {
        return SupportedLanguages.Contains(lang);
    }

    public static async Task<AppLanguage> GetAppLanguage(IJSRuntime js, CancellationToken cancellationToken)
    {
        await _appLanguageSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_appLanguage.HasValue)
            {
                return _appLanguage.Value;
            }

            var cache = await js.Utils().GetStorage("app-language", JavascriptContext.Default.NullableAppLanguage, cancellationToken);

            if (cache.HasValue)
            {
                _appLanguage = cache.Value;
            }
            else
            {
                var code = await js.Window().InvokeAsync<string>("eval", "navigator.language");
                code = code[..2].ToLowerInvariant();

                _appLanguage = ConvertAppLanguage(code, AppLanguage.en);
                await js.Utils().SetStorage("app-language", _appLanguage, JavascriptContext.Default.NullableAppLanguage, cancellationToken);
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

    public static AppLanguage ConvertAppLanguage(string? code, AppLanguage fallback)
    {
        if (code.Empty()) return AppLanguage.en;

        if (System.Enum.TryParse<AppLanguage>(code, true, out var language) && System.Enum.IsDefined(language))
        {
            return language;
        }
        else
            return fallback;
    }

    public static string GetCulture(this NavigationManager navigation)
    {
        var segments = new Uri(navigation.Uri).AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var first = segments.FirstOrDefault()?.ToLowerInvariant();

        return first.IsValidLanguage() ? first! : "en";
    }

    #endregion AppLanguage

    #region DarkMode

    public static Action<bool>? DarkModeChanged { get; set; }

    private static bool? _darkMode;
    private static readonly SemaphoreSlim _darkModeSemaphore = new(1, 1);

    public static async Task<bool?> GetDarkMode(IJSRuntime js, CancellationToken cancellationToken)
    {
        await _darkModeSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_darkMode.HasValue)
            {
                return _darkMode.Value;
            }

            _darkMode = await js.Utils().GetStorage("dark-mode", JavascriptContext.Default.NullableBoolean, cancellationToken);

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

    public static async Task<string?> GetCountry(IpInfoApi api, IJSRuntime js, CancellationToken cancellationToken)
    {
        await _countrySemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_country.NotEmpty())
            {
                return _country;
            }

            var cache = await js.Utils().GetStorage("country", JavascriptContext.Default.String, cancellationToken);

            if (cache.NotEmpty())
            {
                _country = cache.Trim();
            }
            else
            {
                _country = (await api.GetCountry(cancellationToken))?.Trim();

                if (_country.NotEmpty())
                    await js.Utils().SetStorage("country", _country, JavascriptContext.Default.String, cancellationToken);
            }

            return _country;
        }
        catch
        {
            return null;
        }
        finally
        {
            _countrySemaphore.Release();
        }
    }

    #endregion Region Country

    #region Region

    private static Region? _region;
    private static readonly SemaphoreSlim _regionSemaphore = new(1, 1);

    public static ActionDispatcher<Region> RegionChanged { get; } = new();

    public static async Task<Region> GetRegion(IpInfoApi? api = null, IJSRuntime? js = null, CancellationToken cancellationToken = default)
    {
        await _regionSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_region.HasValue)
            {
                return _region.Value;
            }

            var cache = js != null ? await js.Utils().GetStorage("region", JavascriptContext.Default.String, cancellationToken) : null;

            if (cache.NotEmpty())
            {
                _region = ConvertRegion(cache);

                if (_region == null)
                {
                    _region = Region.US;
                    if (js != null) await js.Utils().SetStorage("region", _region, JavascriptContext.Default.NullableRegion, cancellationToken);
                }
            }
            else
            {
                var code = api != null && js != null ? await GetCountry(api, js, cancellationToken) : null;

                _region = ConvertRegion(code) ?? Region.US;
                if (js != null) await js.Utils().SetStorage("region", _region, JavascriptContext.Default.NullableRegion, cancellationToken);
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

        if (System.Enum.TryParse<Region>(code, true, out var region) && System.Enum.IsDefined(region))
            return region;
        else
            return null;
    }

    public static void ChangeRegion(Region value)
    {
        _region = value;
        RegionChanged.Publish(value);
    }

    #endregion Region

    #region ContentLanguage

    private static ContentLanguage? _contentLanguage { get; set; }
    private static readonly SemaphoreSlim _contentLanguageSemaphore = new(1, 1);

    public static async Task<ContentLanguage> GetContentLanguage(IJSRuntime? js = null, CancellationToken cancellationToken = default)
    {
        await _contentLanguageSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_contentLanguage.HasValue)
            {
                return _contentLanguage.Value;
            }

            var cache = js != null ? await js.Utils().GetStorage("content-language", JavascriptContext.Default.NullableContentLanguage, cancellationToken) : null;

            if (cache.HasValue)
            {
                _contentLanguage = cache;
            }
            else
            {
                var code = js != null ? await js.Window().InvokeAsync<string>("eval", "navigator.language") : "en-US";
                code = code.Replace("-", "");

                _contentLanguage = ConvertContentLanguage(code) ?? ContentLanguage.enUS;
                if (js != null) await js.Utils().SetStorage("content-language", _contentLanguage, JavascriptContext.Default.NullableContentLanguage, cancellationToken);
            }

            return _contentLanguage.Value;
        }
        catch
        {
            return ContentLanguage.enUS;
        }
        finally
        {
            _contentLanguageSemaphore.Release();
        }
    }

    public static ContentLanguage? ConvertContentLanguage(string? code)
    {
        if (code.Empty()) return null;

        if (System.Enum.TryParse<ContentLanguage>(code, true, out var language) && System.Enum.IsDefined(language))
            return language;
        else if (code.Length == 2) //few languages have only 2 letter code
        {
            var languages = System.Enum.GetValues<ContentLanguage>();
            foreach (var lang in languages)
            {
                if (lang.ToString().StartsWith(code, StringComparison.InvariantCultureIgnoreCase))
                    return lang;
            }
            return null;
        }
        else
            return null;
    }

    public static void ChangeContentLanguage(ContentLanguage value)
    {
        _contentLanguage = value;
    }

    #endregion ContentLanguage

    public static TaskDispatcher UserStateChanged { get; } = new();
    public static TaskDispatcher ProcessingStarted { get; } = new();
    public static TaskDispatcher ProcessingFinished { get; } = new();
}
