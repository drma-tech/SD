using Blazored.SessionStorage;
using System.Globalization;
using System.Text.Json.Serialization;

namespace SD.WEB.Core
{
    public class Settings
    {
        public Region Region { get; set; }
        public Language Language { get; set; }

        [JsonConstructor]
        public Settings()
        { }

        public Settings(ISyncSessionStorageService session)
        {
            var sett = session.GetItem<Settings>("Settings");

            if (sett == null)
            {
                Enum.TryParse(typeof(Region), RegionInfo.CurrentRegion.Name, out object? region);
                Enum.TryParse(typeof(Language), CultureInfo.CurrentCulture.Name.Replace("-", ""), out object? language);

                Region = (Region?)region ?? Region.US;
                Language = (Language?)language ?? Language.enUS;
            }
            else
            {
                Region = sett.Region;
                Language = sett.Language;
            }
        }
    }
}