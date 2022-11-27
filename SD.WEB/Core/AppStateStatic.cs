using Blazorise;
using System.Globalization;

namespace SD.WEB.Core
{
    public static class AppStateStatic
    {
        public static List<LogContainer> Logs { get; private set; } = new List<LogContainer>();

        public static Region Region { get; private set; }
        public static Language Language { get; private set; }
        public static Bar? Sidebar { get; set; }

        public static Action? RegionChanged { get; set; }
        public static Action? LanguageChanged { get; set; }

        public static bool OnMobile { get; set; }
        public static bool OnTablet { get; set; }
        public static bool OnDesktop { get; set; }
        public static bool OnWidescreen { get; set; }
        public static bool OnFullHD { get; set; }

        static AppStateStatic()
        {
            Enum.TryParse(typeof(Region), RegionInfo.CurrentRegion.Name, out object? region);
            Enum.TryParse(typeof(Language), CultureInfo.CurrentCulture.Name.Replace("-", ""), out object? language);

            Region = (Region?)region ?? Region.US;
            Language = (Language?)language ?? Language.enUS;
        }

        public static void ChangeRegion(Region value)
        {
            Region = value;
            RegionChanged?.Invoke();
        }

        public static void ChangeLanguage(Language value)
        {
            Language = value;
            LanguageChanged?.Invoke();
        }
    }
}