namespace SD.Shared.Models.List
{
    public class Ratings
    {
        public string? imdbId { get; set; }
        public MediaType type { get; set; }
        public string? imdb { get; set; }
        public string? metacritic { get; set; }
        public string? tmdb { get; set; }
        public string? trakt { get; set; }
        public string? rottenTomatoes { get; set; }
        public string? filmAffinity { get; set; }
    }
}