namespace SD.Shared.Models.List.UnifiedMovie
{
    public class Data
    {
        public string? type { get; set; }
        public string? title { get; set; }
        public string? originalTitle { get; set; }
        public string? tagline { get; set; }
        public string? overview { get; set; }
        public string? releaseDate { get; set; }
        public int? runtimeMinutes { get; set; }
        public string? status { get; set; }
        public string? contentRating { get; set; }
        public bool? adult { get; set; }
        public Rating? ratings { get; set; }
        public int? budget { get; set; }
        public int? revenue { get; set; }
        public double? popularity { get; set; }
    }

    public class Rating 
    {
        public string? source { get; set; }
        public double? score { get; set; }
        public double? voteCount { get; set; }
        public List<Rating>? ratings { get; set; }
    }

    public class Root
    {
        public bool success { get; set; }
        public Data? data { get; set; }
    }
}