namespace SD.Shared.Models.List.MoviesRatings2
{
    public class Imdb
    {
        public double? score { get; set; }
        public double? reviewsCount { get; set; }
        public string? url { get; set; }
    }

    public class Letterboxd
    {
        public double? score { get; set; }
        public string? url { get; set; }
    }

    public class Media
    {
        public string? backdrop_path { get; set; }
        public int budget { get; set; }
        public string? homepage { get; set; }
        public int id { get; set; }
        public string? imdb_id { get; set; }
        public List<string>? origin_country { get; set; }
        public string? original_language { get; set; }
        public string? original_title { get; set; }
        public string? overview { get; set; }
        public double? popularity { get; set; }
        public string? poster_path { get; set; }
        public string? release_date { get; set; }
        public int? revenue { get; set; }
        public int? runtime { get; set; }
        public string? status { get; set; }
        public string? tagline { get; set; }
        public string? title { get; set; }
        public bool? video { get; set; }
        public string? director { get; set; }
        public string? media_type { get; set; }
    }

    public class Metacritic
    {
        public int? metascore { get; set; }
        public double? userScore { get; set; }
        public double? averageScore { get; set; }
        public string? url { get; set; }
    }

    public class Ratings
    {
        public Imdb? imdb { get; set; }
        public Metacritic? metacritic { get; set; }
        public RottenTomatoes? rotten_tomatoes { get; set; }
        public Letterboxd? letterboxd { get; set; }
    }

    public class Root
    {
        public string? imdbId { get; set; }
        public long? lastUpdated { get; set; }
        public Media? media { get; set; }
        public Ratings? ratings { get; set; }
        public int? tmdbId { get; set; }
    }

    public class RottenTomatoes
    {
        public int? tomatometer { get; set; }
        public int? tomatometerReviewsCount { get; set; }
        public double? audienceScore { get; set; }
        public int? audienceScoreReviewsCount { get; set; }
        public double? averageScore { get; set; }
        public string? url { get; set; }
    }
}