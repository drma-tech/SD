namespace SD.Shared.Models
{
    public class MediaDetail
    {
        public string? tmdb_id { get; set; }
        public string? title { get; set; }
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

        public List<Video> Videos { get; set; } = new();
        public List<string> Genres { get; set; } = new();
        public List<Collection> Collection { get; set; } = new();

        public MediaType MediaType { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is MediaDetail q && q.tmdb_id == tmdb_id;
        }

        public override int GetHashCode()
        {
            return tmdb_id?.GetHashCode() ?? 0;
        }
    }

    public class Video
    {
        public string? id { get; set; }
        public string? key { get; set; }
        public string? name { get; set; }
    }

    public class Collection
    {
        public string? id { get; set; }
        public int? SeasonNumber { get; set; }
        public string? title { get; set; }
        public DateTime? release_date { get; set; }
        public string? poster_small { get; set; }
    }
}