namespace SD.Shared.Models.List.Tmdb
{
    public class ResultMovieUpcoming
    {
        public string? poster_path { get; set; }
        public bool adult { get; set; }
        public string? overview { get; set; }
        public string? release_date { get; set; }
        public List<int> genre_ids { get; set; } = new();
        public int id { get; set; }
        public string? original_title { get; set; }
        public string? original_language { get; set; }
        public string? title { get; set; }
        public string? backdrop_path { get; set; }
        public double popularity { get; set; }
        public int vote_count { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
    }

    public class DatesMovieUpcoming
    {
        public string? maximum { get; set; }
        public string? minimum { get; set; }
    }

    public class MovieUpcoming
    {
        public int page { get; set; }
        public List<ResultMovieUpcoming> results { get; set; } = new();
        public DatesMovieUpcoming? dates { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}