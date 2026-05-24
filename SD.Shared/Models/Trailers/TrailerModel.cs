namespace SD.Shared.Models.Trailers;

public class TrailerModel
{
    public List<TrailerModelItem> Items { get; set; } = [];
}

public class TrailerModelItem
{
    public TrailerModelItem()
    {
    }

    public TrailerModelItem(string? id, string? title, string? url, string? published, DateTime? dateTime)
    {
        this.id = id;
        this.title = title;
        this.url = url;
        this.published = published;
        this.DateTime = dateTime;
    }

    public string? id { get; set; }
    public string? title { get; set; }
    public string? url { get; set; }
    public string? published { get; set; }
    public DateTime? DateTime { get; set; }
}