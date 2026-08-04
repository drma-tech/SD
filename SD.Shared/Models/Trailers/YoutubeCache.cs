namespace SD.Shared.Models.Trailers;

public class YoutubeCache(string id, TrailerModel data) : CacheDocumentData<TrailerModel>(new CacheIdentity(id), data, TtlCache.SixHours)
{
}

public class TrailerModel
{
    public ICollection<TrailerModelItem> Items { get; set; } = [];
}

public class TrailerModelItem
{
    public TrailerModelItem()
    {
    }

    public TrailerModelItem(string? id, string? title, string? url, string? published, DateTime? dateTime, bool popular)
    {
        this.id = id;
        this.title = title;
        this.url = url;
        this.published = published;
        this.DateTime = dateTime;
        this.Popular = popular;
    }

    public string? id { get; set; }
    public string? title { get; set; }
    public string? url { get; set; }
    public string? published { get; set; }
    public DateTime? DateTime { get; set; }
    public bool Popular { get; set; }
}