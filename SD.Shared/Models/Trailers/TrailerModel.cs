namespace SD.Shared.Models.Trailers
{
    public class TrailerModel
    {
        public List<Item> Items { get; set; } = [];
    }

    public class Item
    {
        public Item()
        {
        }

        public Item(string? id, string? title, string? url)
        {
            this.id = id;
            this.title = title;
            this.url = url;
        }

        public string? id { get; set; }
        public string? title { get; set; }
        public string? url { get; set; }
    }
}