namespace SD.WEB.Modules.Support.Core
{
    public class StoreLink(SD.Shared.Enums.Platform platform, string store, string url, string logo)
    {
        public SD.Shared.Enums.Platform Platform { get; set; } = platform;
        public string Store { get; set; } = store;
        public string Url { get; set; } = url;
        public string Logo { get; set; } = logo;
    }
}