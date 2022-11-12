using System.Globalization;

namespace SD.WEB.Core
{
    public static class Settings
    {
        public static Region Region { get; set; }
        public static Language Language { get; set; }

        static Settings()
        {
            Enum.TryParse(typeof(Region), RegionInfo.CurrentRegion.Name, out object? region);
            Enum.TryParse(typeof(Language), CultureInfo.CurrentCulture.Name.Replace("-", ""), out object? language);

            Region = (Region?)region ?? Region.US;
            Language = (Language?)language ?? Language.enUS;
        }
    }
}