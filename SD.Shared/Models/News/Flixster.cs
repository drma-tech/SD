namespace SD.Shared.Models.News
{
    public class Flixster
    {
        public Data? data { get; set; }
    }

    public class Data
    {
        public List<NewsStory> newsStories { get; set; } = [];
    }

    public class NewsStory
    {
        public string? id { get; set; }
        public string? title { get; set; }
        public MainImage? mainImage { get; set; }
        public string? status { get; set; }
        public string? link { get; set; }
    }

    public class MainImage
    {
        public string? url { get; set; }
    }
}