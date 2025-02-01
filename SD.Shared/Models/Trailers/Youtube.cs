namespace SD.Shared.Models.Trailers
{
    public class Avatar
    {
        public List<Thumbnail> thumbnails { get; set; } = [];
    }

    public class CommandMetadata
    {
        public WebCommandMetadata? webCommandMetadata { get; set; }
    }

    public class Content
    {
        public Video? video { get; set; }
    }

    public class Icon
    {
        public List<Thumbnail> thumbnails { get; set; } = [];
    }

    public class Links
    {
        public List<PrimaryLink> primaryLinks { get; set; } = [];
        public List<SecondaryLink> secondaryLinks { get; set; } = [];
    }

    public class NavigationEndpoint
    {
        public string? clickTrackingParams { get; set; }
        public CommandMetadata? commandMetadata { get; set; }
        public UrlEndpoint? urlEndpoint { get; set; }
    }

    public class PrimaryLink
    {
        public Icon? icon { get; set; }
        public NavigationEndpoint? navigationEndpoint { get; set; }
        public Title? title { get; set; }
    }

    public class Youtube
    {
        public Avatar? avatar { get; set; }
        public List<Content>? contents { get; set; }
        public string? description { get; set; }
        public Links? links { get; set; }
        public string? next { get; set; }
        public string? subscriberCountText { get; set; }
        public string? title { get; set; }
        public string? vanityChannelUrl { get; set; }
        public bool? verified { get; set; }
    }

    public class SecondaryLink
    {
        public Icon? icon { get; set; }
        public NavigationEndpoint? navigationEndpoint { get; set; }
        public Title? title { get; set; }
    }

    public class Thumbnail
    {
        public int? height { get; set; }
        public string? url { get; set; }
        public int? width { get; set; }
    }

    public class Title
    {
        public string? simpleText { get; set; }
    }

    public class UrlEndpoint
    {
        public bool? nofollow { get; set; }
        public string? target { get; set; }
        public string? url { get; set; }
    }

    public class Video
    {
        public string? lengthText { get; set; }
        public string? publishedTimeText { get; set; }
        public List<Thumbnail> thumbnails { get; set; } = [];
        public string? title { get; set; }
        public string? videoId { get; set; }
        public string? viewCountText { get; set; }
    }

    public class WebCommandMetadata
    {
        public int? rootVe { get; set; }
        public string? url { get; set; }
        public string? webPageType { get; set; }
    }
}