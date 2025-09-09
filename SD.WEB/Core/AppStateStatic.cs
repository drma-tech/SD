using SD.Shared.Models.Auth;
using System.Globalization;
using System.Security.Claims;

namespace SD.WEB.Core;

public static class AppStateStatic
{
    static AppStateStatic()
    {
        Language = GetValidLanguage();
    }

    public static bool IsAuthenticated { get; set; }
    public static ClaimsPrincipal? User { get; set; }
    public static string? UserId { get; set; }

    public static List<LogContainer> Logs { get; private set; } = [];

    [Custom(Name = "Region", ResourceType = typeof(GlobalTranslations))]
    public static Region Region { get; private set; } = Region.US;

    [Custom(Name = "Language", ResourceType = typeof(GlobalTranslations))]
    public static Language Language { get; private set; }

    [Custom(Name = "Dark Mode")]
    public static bool DarkMode { get; private set; }

    public static Action? RegionChanged { get; set; }
    public static Action? DarkModeChanged { get; set; }
    public static Action<TempAuthPaddle>? RegistrationSuccessful { get; set; }
    public static Action<string>? ShowError { get; set; }
    public static Action? ProcessingStarted { get; set; }
    public static Action? ProcessingFinished { get; set; }

    public static Region GetValidRegion(string? regionName)
    {
        if (!Enum.TryParse<Region>(regionName, out var region) || !Enum.IsDefined(region))
        {
            regionName = nameof(Region.US);

            _ = Enum.TryParse(regionName, out region);
        }

        return region;
    }

    private static Language GetValidLanguage()
    {
        var cultureName = CultureInfo.CurrentCulture.Name;
        var languageCode = cultureName.Replace("-", "");

        if (!Enum.TryParse<Language>(languageCode, out var language) || !Enum.IsDefined(language))
        {
            var parts = cultureName.Split('-');
            if (parts.Length == 2)
            {
                var region = parts[1];
                language = region switch
                {
                    "001" => Language.enUS, // World
                    "002" => Language.afZA, // Africa
                    "009" => Language.enAU, // Oceania
                    "019" => Language.enUS, // Americas
                    "142" => Language.jaJP, // Asia
                    "150" => Language.frFR, // Europe
                    "419" => Language.esMX, // Latin America
                    _ => Language.enUS
                };
            }
            else
            {
                language = Language.enUS;
            }
        }

        return language;
    }

    public static void ChangeRegion(Region value, bool callAction = true)
    {
        Region = value;
        if (callAction) RegionChanged?.Invoke();
    }

    public static void ChangeLanguage(Language value)
    {
        Language = value;
    }

    public static void ChangeDarkMode(bool darkMode)
    {
        DarkMode = darkMode;
        DarkModeChanged?.Invoke();
    }
}