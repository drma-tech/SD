namespace SD.Shared.Models.List.Tmdb;

public class CastByPerson
{
    public bool? adult { get; set; }
    public string? backdrop_path { get; set; }
    public List<int> genre_ids { get; set; } = [];
    public int id { get; set; }
    public List<string> origin_country { get; set; } = [];
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
    public bool? video { get; set; }
    public double? vote_average { get; set; }
    public int? vote_count { get; set; }
    public string? character { get; set; }
    public string? credit_id { get; set; }
    public int? order { get; set; }
    public int? episode_count { get; set; }
    public string? media_type { get; set; }
}

public class CrewByPerson
{
    public bool? adult { get; set; }
    public string? backdrop_path { get; set; }
    public List<int> genre_ids { get; set; } = [];
    public int id { get; set; }
    public List<string> origin_country { get; set; } = [];
    public string? original_language { get; set; }
    public string? original_title { get; set; }
    public string? original_name { get; set; }
    public string? overview { get; set; }
    public double? popularity { get; set; }
    public string? poster_path { get; set; }
    public string? release_date { get; set; }
    public string? title { get; set; }
    public bool? video { get; set; }
    public string? first_air_date { get; set; }
    public string? name { get; set; }
    public double? vote_average { get; set; }
    public int? vote_count { get; set; }
    public string? credit_id { get; set; }
    public string? department { get; set; }
    public int? episode_count { get; set; }
    public string? job { get; set; }
    public string? media_type { get; set; }
}

public class CreditsByPerson
{
    public List<CastByPerson> cast { get; set; } = [];
    public List<CrewByPerson> crew { get; set; } = [];
    public int id { get; set; }
}