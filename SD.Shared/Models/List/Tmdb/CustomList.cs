namespace SD.Shared.Models.List.Tmdb;

public class ResultCustomList
{
    public int id { get; set; }
    public string? media_type { get; set; }
    public string? overview { get; set; }
    public string? poster_path { get; set; }
    public string? release_date { get; set; }
    public string? first_air_date { get; set; }
    public string? title { get; set; }
    public string? name { get; set; }
    public double vote_average { get; set; }
    public int vote_count { get; set; }
}

public class CustomList
{
    public IDictionary<string, string>? comments { get; set; }
    public IReadOnlyCollection<ResultCustomList> results { get; set; } = [];
    public int total_pages { get; set; }
}