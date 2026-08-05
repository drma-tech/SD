namespace SD.Shared.Models;

public class MediaDetail : EqualityBase<MediaDetail>
{
    public string? tmdb_id { get; set; }
    public string? title { get; set; }
    public string? original_title { get; set; }
    public string? original_language { get; set; }
    public string? plot { get; set; }
    public DateTime? release_date { get; set; }
    public string? poster_small { get; set; }
    public string? poster_large { get; set; }
    public double rating { get; set; }
    public int? runtime { get; set; }
    public string? homepage { get; set; }
    public string? comments { get; set; }

    public int? collectionId { get; set; }
    public string? collectionName { get; set; }
    public string? collectionLogo { get; set; }

    public IEnumerable<Video> Videos { get; set; } = [];
    public IEnumerable<string> Genres { get; set; } = [];
    public ICollection<Collection> Collection { get; set; } = [];

    public MediaType MediaType { get; set; }

    protected override object?[] EqualityValues => [tmdb_id];
}

public class Video
{
    public string? id { get; set; }
    public string? key { get; set; }
    public string? name { get; set; }
    public string? type { get; set; }
}

public class Collection
{
    public string? id { get; set; }
    public int? SeasonNumber { get; set; }
    public string? title { get; set; }
    public DateTime? release_date { get; set; }
    public string? poster_small { get; set; }
}