using System.ComponentModel.DataAnnotations.Schema;

namespace SD.Shared.Models.List
{
    public class Ratings
    {
        public string? imdbId { get; set; }
        public MediaType type { get; set; }
        public string? imdb { get; set; }
        public string? metacritic { get; set; }
        public string? tmdb { get; set; }
        public string? trakt { get; set; }

        /// <summary>
        /// TODO: error -> Access Denied
        /// </summary>
        [NotMapped]
        public string? rottenTomatoes { get; set; }

        /// <summary>
        /// TODO: link uses its own code
        /// </summary>
        [NotMapped]
        public string? filmAffinity { get; set; }
    }
}