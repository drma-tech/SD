using Blazorise;
using Microsoft.AspNetCore.Components;

namespace SD.WEB.Core
{
    public static class RefreshCore
    {
        public static EventCallback<Region> RegionChanged { get; set; }

        public static EventCallback<Language> LanguageChanged { get; set; }

        public static Bar? Sidebar { get; set; }

        public static async Task ChangeRegion(Region value)
        {
            await RegionChanged.InvokeAsync(value);
        }

        public static async Task ChangeLanguage(Language value)
        {
            await LanguageChanged.InvokeAsync(value);
        }
    }
}