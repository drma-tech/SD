namespace SD.Shared.Models.Reviews
{
    public class ReviewModel
    {
        public List<Item> Items { get; set; } = [];
    }

    public class Item
    {
        public Item()
        {
        }

        public Item(string? reviewSite, string? reviewUrl, string? reviewer, int? score, string? quote)
        {
            this.reviewSite = reviewSite;
            this.reviewUrl = reviewUrl;
            this.reviewer = reviewer;
            this.score = score;
            this.quote = quote;
        }

        public string? reviewSite { get; set; }
        public string? reviewUrl { get; set; }
        public string? reviewer { get; set; }
        public int? score { get; set; }
        public string? quote { get; set; }
    }
}