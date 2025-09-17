using MudBlazor;
using SD.Shared.Models.Auth;
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

    public static Platform Platform { get; set; } = Platform.webapp;
    public static string? Version { get; set; }

    #region AppLanguage

    [Custom(Name = "AppLanguage", ResourceType = typeof(GlobalTranslations))]
    public static AppLanguage AppLanguage { get; set; }

    public static AppLanguage GetValidAppLanguage(CultureInfo? culture)
    {
        culture ??= CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture;
        var code = culture.TwoLetterISOLanguageName?.ToLowerInvariant();

        if (Enum.TryParse<AppLanguage>(code, true, out var language) && Enum.IsDefined(language))
        {
            return language;
        }
        else
        {
            return AppLanguage.en;
        }
    }

    #endregion AppLanguage

    #region ContentLanguage

    [Custom(Name = "ContentLanguage", ResourceType = typeof(GlobalTranslations))]
    public static ContentLanguage ContentLanguage { get; set; }

    public static Action<ContentLanguage>? ContentLanguageChanged { get; set; }

    public static ContentLanguage GetValidContentLanguage(CultureInfo? culture)
    {
        culture ??= CultureInfo.CurrentUICulture ?? CultureInfo.CurrentCulture;
        var parts = culture.Name.Split('-'); // ex: ["pt","BR"], ["es","419"], ["sr","Cyrl"]

        var combined = string.Join("", parts);

        if (Enum.TryParse<ContentLanguage>(combined, true, out var lang) && Enum.IsDefined(lang))
        {
            return lang;
        }
        else if (parts.Length == 1)
        {
            return parts[0] switch
            {
                "en" => ContentLanguage.enUS,
                "pt" => ContentLanguage.ptBR,
                "fr" => ContentLanguage.frFR,
                "es" => ContentLanguage.esES,
                "de" => ContentLanguage.deDE,
                "ja" => ContentLanguage.jaJP,
                "zh" => ContentLanguage.zhCN,
                _ => ContentLanguage.enUS
            };
        }
        else if (parts.Length > 1)
        {
            return parts[1] switch
            {
                "001" => ContentLanguage.enUS, // World
                "002" => ContentLanguage.afZA, // Africa
                "009" => ContentLanguage.enAU, // Oceania
                "019" => ContentLanguage.enUS, // Americas
                "142" => ContentLanguage.jaJP, // Asia
                "150" => ContentLanguage.frFR, // Europe
                "419" => ContentLanguage.esMX, // Latin America

                "Cyrl" => ContentLanguage.srRS, // Serbian (Cyrillic)
                "Latn" => ContentLanguage.srRS, // Serbian (Latin)
                "Hant" => ContentLanguage.zhTW, // Traditional Chinese
                "Hans" => ContentLanguage.zhCN, // Simplified Chinese

                _ => ContentLanguage.enUS
            };
        }
        else
        {
            return ContentLanguage.enUS;
        }
    }

    public static void ChangeContentLanguage(ContentLanguage value)
    {
        ContentLanguage = value;
        ContentLanguageChanged?.Invoke(value);
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

    [Custom(Name = "DarkMode", ResourceType = typeof(GlobalTranslations))]
    public static bool DarkMode { get; private set; }

    public static Action<bool>? DarkModeChanged { get; set; }

    public static void ChangeDarkMode(bool darkMode)
    {
        DarkMode = darkMode;
        DarkModeChanged?.Invoke(darkMode);
    }

    #endregion DarkMode

    public static Action<TempAuthPaddle>? RegistrationSuccessful { get; set; }
    public static Action<string>? ShowError { get; set; }
    public static Action? ProcessingStarted { get; set; }
    public static Action? ProcessingFinished { get; set; }
}