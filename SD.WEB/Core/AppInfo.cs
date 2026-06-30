using SD.WEB.Modules.Support.Core;

namespace SD.WEB.Core
{
    public static class AppInfo
    {
        public static string CompanyName { get; set; } = "DRMA Tech";
        public static string CompanyWebSite { get; set; } = $"https://www.drma-tech.com";

        public static string Title { get; set; } = "Streaming Discovery";
        public static string Domain { get; set; } = "streamingdiscovery";
        public static string WebSite { get; set; } = $"https://{Domain}.com";
        public static int Year { get; set; } = 2021;

        public static readonly string? _windowsId = "9pb1pkrdd8l0";
        public static readonly string? _googleId = "com.streamingdiscovery.www.twa";
        public static readonly string? _appleId = "id6749285238";
        public static readonly string? _huaweiId = "C111325113";
        public static readonly string? _xiaomiId = "com.streamingdiscovery.www.twa";
        public static readonly string? _amazonId = "B0CL3KXKD9";

        public static readonly StoreLink[] Stores =
        [
            new(Platform.windows, "Microsoft Store", $"https://apps.microsoft.com/detail/{_windowsId}", "/logo/microsoft-store.webp" ),
            new(Platform.play, "Google Play", $"https://play.google.com/store/apps/details?id={_googleId}", "/logo/google-play.webp" ),
            new(Platform.ios, "App Store", $"https://apps.apple.com/us/app/{_appleId}", "/logo/app-store.webp" ),
            new(Platform.huawei, "Huawei AppGallery", $"https://appgallery.huawei.com/app/{_huaweiId}", "/logo/huawei.webp" ),
            new(Platform.xiaomi, "Xiaomi GetApps", $"https://global.app.mi.com/details?id={_xiaomiId}", "/logo/xiaomi.webp" ),
            new(Platform.amazon, "Amazon Appstore", $"https://www.amazon.com/gp/product/{_amazonId}", "/logo/amazon.webp" )
        ];

        public static readonly ProductLink[] Products =
        [
            //new("Streaming Discovery", "Discover Movies and Series on Streaming Platforms", "https://streamingdiscovery.com", "/logo/streamingdiscovery.webp", true ),
            new("Modern Matchmaker", "Find a compatible partner through Smart Matchmaking", "https://modern-matchmaker.com", "/logo/modern-matchmaker.webp", true ),
            new("My Next Spot", "Find the Best Cities and Countries to Live or Travel", "https://my-next-spot.com", "/logo/next-spot.webp", true ),
            new("Web Standards", "Web Standards Generator for Websites and PWAs", "https://web-standards.com", "/logo/webstandards.webp", false ),
       ];
    }
}
