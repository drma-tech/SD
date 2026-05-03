using SD.WEB.Modules.Support.Core;

namespace SD.WEB.Core
{
    public static class AppInfo
    {
        public static string CompanyName { get; set; } = "DRMA Tech";
        public static string CompanyWebSite { get; set; } = $"https://www.drma-tech.com";

        public static string Title { get; set; } = "Streaming Discovery";
        public static string Subtitle { get; set; } = "Discover Movies and Series on Streaming Platforms";
        public static string Domain { get; set; } = "streamingdiscovery";
        public static string WebSite { get; set; } = $"https://www.{Domain}.com";
        public static int Year { get; set; } = 2021;

        public static readonly string? _windowsId = "9pb1pkrdd8l0";
        public static readonly string? _googleId = "com.streamingdiscovery.www.twa";
        public static readonly string? _appleId = "id6749285238";
        public static readonly string? _huaweiId = "C111325113";
        public static readonly string? _xiaomiId = "com.streamingdiscovery.www.twa";
        public static readonly string? _amazonId = "B0CL3KXKD9";

        public static readonly StoreLink[] Stores =
        [
            new(Platform.windows, "Microsoft Store", $"https://apps.microsoft.com/detail/{_windowsId}", "/logo/microsoft-store.png" ),
            new(Platform.play, "Google Play", $"https://play.google.com/store/apps/details?id={_googleId}", "/logo/google-play.png" ),
            new(Platform.ios, "App Store", $"https://apps.apple.com/us/app/{_appleId}", "/logo/app-store.png" ),
            new(Platform.huawei, "Huawei AppGallery", $"https://appgallery.huawei.com/app/{_huaweiId}", "/logo/huawei.png" ),
            new(Platform.xiaomi, "Xiaomi GetApps", $"https://global.app.mi.com/details?id={_xiaomiId}", "/logo/xiaomi.png" ),
            new(Platform.amazon, "Amazon Appstore", $"https://www.amazon.com/gp/product/{_amazonId}", "/logo/amazon.png" )
        ];

        public static readonly ProductLink[] Products =
        [
            //new("Streaming Discovery", "Manage, Track, Discover", "https://www.streamingdiscovery.com", "/logo/streamingdiscovery.png", true ),
            new("Modern Matchmaker", "Find your life partner", "https://www.modern-matchmaker.com", "/logo/modern-matchmaker.png", true ),
            new("My Next Spot", "Match your next destination", "https://www.my-next-spot.com", "/logo/next-spot.png", false ),
            new("WebStandards", "Web Developer Tools", "https://www.web-standards.com", "/logo/webstandards.png", false ),
       ];
    }
}