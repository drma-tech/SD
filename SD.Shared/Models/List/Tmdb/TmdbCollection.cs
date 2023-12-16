using System.Text.Json.Serialization;

namespace SD.Shared.Models.List.Tmdb
{
    public class Part
    {
        public bool adult { get; set; }
        public string? backdrop_path { get; set; }
        public List<int> genre_ids { get; set; } = [];
        public int id { get; set; }
        public string? original_language { get; set; }
        public string? original_title { get; set; }
        public string? overview { get; set; }
        public double popularity { get; set; }
        public string? poster_path { get; set; }
        public string? release_date { get; set; }
        public string? title { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }

        [JsonIgnore]
        public int? season_number { get; set;}
    }

    public class TmdbCollection
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? overview { get; set; }
        public string? poster_path { get; set; }
        public string? backdrop_path { get; set; }
        public List<Part> parts { get; set; } = [];
    }
}