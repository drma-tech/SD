using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SD.Shared.Modal.Tmdb
{
    //public class Comments
    //{
    //    [JsonProperty("movie:12162")]
    //    public string Movie12162 { get; set; }

    //    [JsonProperty("movie:194662")]
    //    public object Movie194662 { get; set; }

    //    [JsonProperty("movie:314365")]
    //    public object Movie314365 { get; set; }

    //    [JsonProperty("movie:376867")]
    //    public object Movie376867 { get; set; }

    //    [JsonProperty("movie:399055")]
    //    public string Movie399055 { get; set; }

    //    [JsonProperty("movie:45269")]
    //    public object Movie45269 { get; set; }

    //    [JsonProperty("movie:490132")]
    //    public string Movie490132 { get; set; }

    //    [JsonProperty("movie:496243")]
    //    public string Movie496243 { get; set; }

    //    [JsonProperty("movie:581734")]
    //    public string Movie581734 { get; set; }

    //    [JsonProperty("movie:68734")]
    //    public object Movie68734 { get; set; }

    //    [JsonProperty("movie:74643")]
    //    public object Movie74643 { get; set; }

    //    [JsonProperty("movie:76203")]
    //    public object Movie76203 { get; set; }
    //}

    public class CreatedByCustomListNew
    {
        public string gravatar_hash { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
    }

    //public class ObjectIds
    //{
    //    [JsonProperty("movie:12162")]
    //    public string Movie12162 { get; set; }

    //    [JsonProperty("movie:194662")]
    //    public string Movie194662 { get; set; }

    //    [JsonProperty("movie:314365")]
    //    public string Movie314365 { get; set; }

    //    [JsonProperty("movie:376867")]
    //    public string Movie376867 { get; set; }

    //    [JsonProperty("movie:399055")]
    //    public string Movie399055 { get; set; }

    //    [JsonProperty("movie:45269")]
    //    public string Movie45269 { get; set; }

    //    [JsonProperty("movie:490132")]
    //    public string Movie490132 { get; set; }

    //    [JsonProperty("movie:496243")]
    //    public string Movie496243 { get; set; }

    //    [JsonProperty("movie:581734")]
    //    public string Movie581734 { get; set; }

    //    [JsonProperty("movie:68734")]
    //    public string Movie68734 { get; set; }

    //    [JsonProperty("movie:74643")]
    //    public string Movie74643 { get; set; }

    //    [JsonProperty("movie:76203")]
    //    public string Movie76203 { get; set; }
    //}

    public class ResultCustomListNew
    {
        public bool adult { get; set; }
        public string backdrop_path { get; set; }
        public List<int> genre_ids { get; set; }
        public int id { get; set; }
        public string media_type { get; set; }
        public string original_language { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public double popularity { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public string first_air_date { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public bool video { get; set; }
        public double vote_average { get; set; }
        public int vote_count { get; set; }
    }

    public class CustomListNew
    {
        public double average_rating { get; set; }
        public object backdrop_path { get; set; }
        public JsonElement comments { get; set; }
        public CreatedByCustomListNew created_by { get; set; }
        public string description { get; set; }
        public int id { get; set; }
        public string iso_3166_1 { get; set; }
        public string iso_639_1 { get; set; }
        public string name { get; set; }
        public JsonElement object_ids { get; set; }
        public int page { get; set; }
        public object poster_path { get; set; }
        public bool @public { get; set; }
        public List<ResultCustomListNew> results { get; set; }
        public long revenue { get; set; }
        public int runtime { get; set; }
        public string sort_by { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}
