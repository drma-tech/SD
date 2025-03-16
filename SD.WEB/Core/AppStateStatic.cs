using SD.Shared.Models.Auth;
using System.Globalization;

namespace SD.WEB.Core
{
    public static class AppStateStatic
    {
        public static List<LogContainer> Logs { get; private set; } = [];

        [Custom(Name = "Region", ResourceType = typeof(GlobalTranslations))]
        public static Region Region { get; private set; }

        [Custom(Name = "Language", ResourceType = typeof(GlobalTranslations))]
        public static Language Language { get; private set; }

        public static Action? RegionChanged { get; set; }
        public static Action<TempClientePaddle>? RegistrationSuccessful { get; set; }
        public static Action<string>? ShowError { get; set; }
        public static Action<bool>? ShowFeedback { get; set; }

        static AppStateStatic()
        {
            Enum.TryParse(typeof(Region), RegionInfo.CurrentRegion.Name, out object? region);
            Enum.TryParse(typeof(Language), CultureInfo.CurrentCulture.Name.Replace("-", ""), out object? language);

            Region = (Region?)region ?? Region.US;
            Language = (Language?)language ?? Language.enUS;
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