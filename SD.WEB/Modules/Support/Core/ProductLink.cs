namespace SD.WEB.Modules.Support.Core
{
    public class ProductLink(string title, string subTitle, string url, string logo)
    {
        public string Title { get; set; } = title;
        public string SubTitle { get; set; } = subTitle;
        public string Url { get; set; } = url;
        public string Logo { get; set; } = logo;
    }
}
