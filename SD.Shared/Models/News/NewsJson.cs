namespace SD.Shared.Models.News
{
    public class ArticleTitle
    {
        public string? plainText { get; set; }
    }

    public class Data
    {
        public News? news { get; set; }
    }

    public class Edge
    {
        public Node? node { get; set; }
    }

    public class Image
    {
        public string? id { get; set; }
        public string? url { get; set; }
        public int? height { get; set; }
        public int? width { get; set; }
    }

    public class News
    {
        public List<Edge>? edges { get; set; }
        public int? total { get; set; }
    }

    public class Node
    {
        public string? id { get; set; }
        public string? byline { get; set; }
        public DateTime? date { get; set; }
        public string? externalUrl { get; set; }
        public ArticleTitle? articleTitle { get; set; }
        public Image? image { get; set; }
    }

    public class NewsJson
    {
        public Data? data { get; set; }
    }
}