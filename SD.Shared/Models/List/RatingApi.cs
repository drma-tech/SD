using System.Text.Json.Serialization;

namespace SD.Shared.Models.List;

public class AlternativeTitle
{
    public string? title { get; set; }

    [JsonPropertyName("alpha-2")] public string? alpha2 { get; set; }

    [JsonPropertyName("alpha-3")] public string? alpha3 { get; set; }
}

public class Audience
{
    public double rating { get; set; }
    public int count { get; set; }
    public int bestValue { get; set; }
}

public class Critics
{
    public int rating { get; set; }
    public int count { get; set; }
    public int bestValue { get; set; }
}

public class FilmAffinity
{
    public Audience? audience { get; set; }
}

public class Ids
{
    public string? AllMovie { get; set; }

    [JsonPropertyName("Amazon Prime")] public string? AmazonPrime { get; set; }

    [JsonPropertyName("American Film Institute")]
    public string? AmericanFilmInstitute { get; set; }

    [JsonPropertyName("Apple TV")] public string? AppleTV { get; set; }

    public string? Britannica { get; set; }

    [JsonPropertyName("Comic Vine")] public string? ComicVine { get; set; }

    public string? FilmAffinity { get; set; }

    [JsonPropertyName("Google Knowledge Graph")]
    public string? GoogleKnowledgeGraph { get; set; }

    [JsonPropertyName("Google Play")] public string? GooglePlay { get; set; }

    public string? IMDb { get; set; }
    public string? Letterboxd { get; set; }
    public string? Metacritic { get; set; }

    [JsonPropertyName("Movie Review Query Engine")]
    public string? MovieReviewQueryEngine { get; set; }

    public string? Netflix { get; set; }
    public string? Plex { get; set; }

    [JsonPropertyName("Rotten Tomatoes")] public string? RottenTomatoes { get; set; }

    public string? TCM { get; set; }
    public string? TMDB { get; set; }

    [JsonPropertyName("TV Tropes")] public string? TVTropes { get; set; }

    [JsonPropertyName("The Numbers")] public string? TheNumbers { get; set; }

    public string? Trakt { get; set; }
    public string? Wikidata { get; set; }
    public string? Wikipedia { get; set; }
    public string? iTunes { get; set; }
}

public class IMDb
{
    public Audience? audience { get; set; }
}

public class Letterboxd
{
    public Audience? audience { get; set; }
}

public class Links
{
    public string? AllMovie { get; set; }

    [JsonPropertyName("Amazon Prime")] public string? AmazonPrime { get; set; }

    [JsonPropertyName("American Film Institute")]
    public string? AmericanFilmInstitute { get; set; }

    [JsonPropertyName("Apple TV")] public string? AppleTV { get; set; }

    public string? Britannica { get; set; }

    [JsonPropertyName("Comic Vine")] public string? ComicVine { get; set; }

    public string? FilmAffinity { get; set; }

    [JsonPropertyName("Google Knowledge Graph")]
    public string? GoogleKnowledgeGraph { get; set; }

    [JsonPropertyName("Google Play")] public string? GooglePlay { get; set; }

    public string? IMDb { get; set; }
    public string? Letterboxd { get; set; }
    public string? Metacritic { get; set; }

    [JsonPropertyName("Movie Review Query Engine")]
    public string? MovieReviewQueryEngine { get; set; }

    public string? Netflix { get; set; }

    [JsonPropertyName("Official site")] public string? Officialsite { get; set; }

    public string? Plex { get; set; }

    [JsonPropertyName("Rotten Tomatoes")] public string? RottenTomatoes { get; set; }

    public string? TCM { get; set; }
    public string? TMDB { get; set; }

    [JsonPropertyName("TV Tropes")] public string? TVTropes { get; set; }

    [JsonPropertyName("The Numbers")] public string? TheNumbers { get; set; }

    public string? Trakt { get; set; }
    public string? Wikidata { get; set; }
    public string? Wikipedia { get; set; }
    public string? iTunes { get; set; }
}

public class Metacritic
{
    public Audience? audience { get; set; }
    public Critics? critics { get; set; }
}

public class Origin
{
    public string? name { get; set; }

    [JsonPropertyName("alpha-2")] public string? alpha2 { get; set; }

    [JsonPropertyName("alpha-3")] public string? alpha3 { get; set; }
}

public class RatingsApi
{
    public FilmAffinity? FilmAffinity { get; set; }
    public IMDb? IMDb { get; set; }
    public Letterboxd? Letterboxd { get; set; }
    public Metacritic? Metacritic { get; set; }

    [JsonPropertyName("Rotten Tomatoes")] public RottenTomatoes? RottenTomatoes { get; set; }

    public TMDB? TMDB { get; set; }
}

public class Result
{
    public string? title { get; set; }
    public string? type { get; set; }
    public string? year { get; set; }
    public string? date { get; set; }
    public string? overview { get; set; }
    public object? crew { get; set; }
    public List<AlternativeTitle> alternative_titles { get; set; } = [];
    public List<Origin> origins { get; set; } = [];
    public List<string> languages { get; set; } = [];
    public List<string> genres { get; set; } = [];

    [JsonPropertyName("Ratings")] public RatingsApi? ratings { get; set; }

    public Ids? ids { get; set; }
    public Links? links { get; set; }
    public object? reviews { get; set; }
}

public class RatingApiRoot
{
    public string? status { get; set; }
    public Result? result { get; set; }
}

public class RottenTomatoes
{
    public Audience? audience { get; set; }
    public Critics? critics { get; set; }
}

public class TMDB
{
    public Audience? audience { get; set; }
}