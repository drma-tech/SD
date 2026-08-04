namespace SD.Shared.Models.List.Tmdb;

public class TvResultFindByImdb
{
    public string? backdrop_path { get; set; }
    public IReadOnlyCollection<int> genre_ids { get; set; } = [];
    public string? original_language { get; set; }
    public string? poster_path { get; set; }
    public int vote_count { get; set; }
    public double vote_average { get; set; }
    public string? name { get; set; }
    public string? first_air_date { get; set; }
    public string? overview { get; set; }
    public string? original_name { get; set; }
    public IReadOnlyCollection<string> origin_country { get; set; } = [];
    public int id { get; set; }
    public double popularity { get; set; }
}

public class MovieResult
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

public class PersonResult
{
    public bool adult { get; set; }
    public int id { get; set; }
    public string? name { get; set; }
    public string? original_name { get; set; }
    public string? media_type { get; set; }
    public double popularity { get; set; }
    public int gender { get; set; }
    public string? known_for_department { get; set; }
    public string? profile_path { get; set; }
    public IReadOnlyCollection<KnownFor> known_for { get; set; } = [];
}

public class FindByImdb
{
    public IReadOnlyCollection<MovieResult> movie_results { get; set; } = [];
    public IReadOnlyCollection<PersonResult>? person_results { get; set; } = [];
    public IReadOnlyCollection<TvResultFindByImdb> tv_results { get; set; } = [];
    public IReadOnlyCollection<object>? tv_episode_results { get; set; }
    public IReadOnlyCollection<object>? tv_season_results { get; set; }
}