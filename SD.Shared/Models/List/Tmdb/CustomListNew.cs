using System.Text.Json;

namespace SD.Shared.Model.List.Tmdb
{
    public class CreatedByCustomListNew
    {
        public string? gravatar_hash { get; set; }
        public string? id { get; set; }
        public string? name { get; set; }
        public string? username { get; set; }
    }

    public class ResultCustomListNew
    {
        public bool adult { get; set; }
        public string? backdrop_path { get; set; }
        public List<int> genre_ids { get; set; } = new();
        public int id { get; set; }
        public string? media_type { get; set; }
        public string? original_language { get; set; }
        public string? original_title { get; set; }
        public string? overview { get; set; }
        public double popularity { get; set; }
        public string? poster_path { get; set; }
        public string? release_date { get; set; }
        public string? first_air_date { get; set; }
        public string? title { get; set; }
        public string? name { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
    }

    public class CustomListNew
    {
        public double average_rating { get; set; }
        public object? backdrop_path { get; set; }
        public JsonElement comments { get; set; }
        public CreatedByCustomListNew? created_by { get; set; }
        public string? description { get; set; }
        public int id { get; set; }
        public string? iso_3166_1 { get; set; }
        public string? iso_639_1 { get; set; }
        public string? name { get; set; }
        public JsonElement object_ids { get; set; }
        public int page { get; set; }
        public object? poster_path { get; set; }
        public bool @public { get; set; }
        public List<ResultCustomListNew> results { get; set; } = new();
        public long revenue { get; set; }
        public int runtime { get; set; }
        public string? sort_by { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}