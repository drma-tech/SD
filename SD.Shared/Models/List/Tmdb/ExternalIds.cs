namespace SD.Shared.Models.List.Tmdb;

public class MovieExternalIds
{
    public int id { get; set; }
    public string? imdb_id { get; set; }
    public string? wikidata_id { get; set; }
    public string? facebook_id { get; set; }
    public string? instagram_id { get; set; }
    public string? twitter_id { get; set; }
}

public class ShowExternalIds
{
    public int id { get; set; }
    public string? imdb_id { get; set; }
    public string? freebase_mid { get; set; }
    public string? freebase_id { get; set; }
    public int? tvdb_id { get; set; }
    public int? tvrage_id { get; set; }
    public string? wikidata_id { get; set; }
    public string? facebook_id { get; set; }
    public string? instagram_id { get; set; }
    public string? twitter_id { get; set; }
}