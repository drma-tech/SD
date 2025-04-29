using SD.Shared.Models.Auth;
using System.Globalization;

namespace SD.WEB.Core
{
    public static class AppStateStatic
    {
        public static List<LogContainer> Logs { get; private set; } = [];

        [Custom(Name = "Region", ResourceType = typeof(GlobalTranslations))]
        public static Region Region { get; private set; } = Region.US;

        [Custom(Name = "Language", ResourceType = typeof(GlobalTranslations))]
        public static Language Language { get; private set; } = Language.enUS;

        public static Action? RegionChanged { get; set; }
        public static Action<TempClientePaddle>? RegistrationSuccessful { get; set; }
        public static Action<string>? ShowError { get; set; }

        static AppStateStatic()
        {
            Region = GetValidRegion();
            Language = GetValidLanguage();
        }

        private static Region GetValidRegion()
        {
            var regionName = RegionInfo.CurrentRegion.Name;

            if (!Enum.TryParse<Region>(regionName, out var region) || !Enum.IsDefined(region))
            {
                regionName = regionName switch
                {
                    "001" => Region.US.ToString(), // World
                    "002" => Region.ZA.ToString(), // Africa
                    "009" => Region.AU.ToString(), // Oceania
                    "019" => Region.US.ToString(), // Americas
                    "142" => Region.JP.ToString(), // Asia
                    "150" => Region.FR.ToString(), // Europe
                    "419" => Region.MX.ToString(), // Latin America
                    _ => Region.US.ToString()
                };

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
    }
}