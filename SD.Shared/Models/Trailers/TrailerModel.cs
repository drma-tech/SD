namespace SD.Shared.Models.Trailers;

public class TrailerModel
{
    public List<Item> Items { get; set; } = [];
}

public class Item
{
    public Item()
    {
    }

    public Item(string? id, string? title, string? url, string? published)
    {
        this.id = id;
        this.title = title;
        this.url = url;
        this.published = published;
    }

    public string? id { get; set; }
    public string? title { get; set; }
    public string? url { get; set; }
    public string? published { get; set; }
}