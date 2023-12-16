namespace SD.Shared.Models.Reviews
{
    public class Image
    {
        public int? height { get; set; }
        public string? id { get; set; }
        public string? url { get; set; }
        public int? width { get; set; }
    }

    public class Review
    {
        public string? quote { get; set; }
        public string? reviewSite { get; set; }
        public string? reviewUrl { get; set; }
        public string? reviewer { get; set; }
        public int? score { get; set; }
    }

    public class MetaCritic
    {
        //[JsonProperty("@type")]
        public string? @type { get; set; }

        public string? id { get; set; }
        public int? metaScore { get; set; }
        public string? metacriticUrl { get; set; }
        public int? reviewCount { get; set; }
        public int? userRatingCount { get; set; }
        public double? userScore { get; set; }
        public List<Review> reviews { get; set; } = [];
        public Title? title { get; set; }
    }

    public class Title
    {
        public string? id { get; set; }
        public Image? image { get; set; }
        public int? runningTimeInMinutes { get; set; }
        public string? title { get; set; }
        public string? titleType { get; set; }
        public int? year { get; set; }
    }
}