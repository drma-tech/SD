namespace SD.Shared.Models.List;

public class Ratings
{
    public string? imdbId { get; set; }
    public MediaType type { get; set; }
    public string? imdb { get; set; }
    public string? imdbLink { get; set; }
    public string? metacritic { get; set; }
    public string? metacriticLink { get; set; }
    public string? tmdb { get; set; }
    public string? tmdbLink { get; set; }
    public string? trakt { get; set; }
    public string? traktLink { get; set; }
    public string? rottenTomatoes { get; set; }
    public string? rottenTomatoesLink { get; set; }
    public string? filmAffinity { get; set; }
    public string? filmAffinityLink { get; set; }
    public string? letterboxd { get; set; }
    public string? letterboxdLink { get; set; }
}