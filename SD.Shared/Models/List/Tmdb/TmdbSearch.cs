namespace SD.Shared.Models.List.Tmdb;

public class KnownFor
{
    public bool adult { get; set; }
    public string? backdrop_path { get; set; }
    public int id { get; set; }
    public string? title { get; set; }
    public string? original_language { get; set; }
    public string? original_title { get; set; }
    public string? overview { get; set; }
    public string? poster_path { get; set; }
    public string? media_type { get; set; }
    public IReadOnlyCollection<int> genre_ids { get; set; } = [];
    public double popularity { get; set; }
    public string? release_date { get; set; }
    public bool video { get; set; }
    public double vote_average { get; set; }
    public int vote_count { get; set; }
}

public class TmdbResult
{
    public bool? adult { get; set; }
    public string? backdrop_path { get; set; }
    public string? media_type { get; set; }
    public IReadOnlyCollection<int> genre_ids { get; set; } = [];
    public IReadOnlyCollection<string> origin_country { get; set; } = [];
    public int id { get; set; }
    public string? original_language { get; set; }
    public string? original_title { get; set; }
    public string? original_name { get; set; }
    public string? overview { get; set; }
    public double? popularity { get; set; }
    public string? poster_path { get; set; }
    public string? release_date { get; set; }
    public string? first_air_date { get; set; }
    public string? title { get; set; }
    public string? name { get; set; }
    public string? known_for_department { get; set; }
    public string? profile_path { get; set; }
    public IReadOnlyCollection<KnownFor> known_for { get; set; } = [];
    public bool? video { get; set; }
    public double vote_average { get; set; }
    public int vote_count { get; set; }
}

public class TmdbSearchMulti
{
    public int page { get; set; }
    public IReadOnlyCollection<TmdbResult> results { get; set; } = [];
    public int total_pages { get; set; }
    public int total_results { get; set; }
}

public class TmdbResultKeyword
{
    public int id { get; set; }
    public string? name { get; set; }
}

public class TmdbSearchKeyword
{
    public int page { get; set; }
    public IReadOnlyCollection<TmdbResultKeyword> results { get; set; } = [];
    public int total_pages { get; set; }
    public int total_results { get; set; }
}

public class TmdbMovieKeyword
{
    public int id { get; set; }
    public IReadOnlyCollection<TmdbResultKeyword> keywords { get; set; } = [];
}

public class TmdbSerieKeyword
{
    public int id { get; set; }
    public IReadOnlyCollection<TmdbResultKeyword> results { get; set; } = [];
}