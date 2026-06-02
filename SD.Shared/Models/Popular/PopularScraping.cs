namespace SD.Shared.Models.Popular
{
    public class ProductionCompany
    {
        public string? id { get; set; }
        public string? name { get; set; }
    }

    public class PopularScraping
    {
        public string? id { get; set; }
        public string? url { get; set; }
        public string? primaryTitle { get; set; }
        public string? originalTitle { get; set; }
        public string? type { get; set; }
        public string? description { get; set; }
        public string? primaryImage { get; set; }
        public List<Thumbnail>? thumbnails { get; set; }
        public string? trailer { get; set; }
        public string? contentRating { get; set; }
        public long? startYear { get; set; }
        public long? endYear { get; set; }
        public string? releaseDate { get; set; }
        public List<string>? interests { get; set; }
        public List<string>? countriesOfOrigin { get; set; }
        public List<string>? externalLinks { get; set; }
        public List<string>? spokenLanguages { get; set; }
        public List<string>? filmingLocations { get; set; }
        public List<ProductionCompany>? productionCompanies { get; set; }
        public long? budget { get; set; }
        public long? grossWorldwide { get; set; }
        public List<string>? genres { get; set; }
        public bool isAdult { get; set; }
        public long? runtimeMinutes { get; set; }
        public double? averageRating { get; set; }
        public long? numVotes { get; set; }
        public long? metascore { get; set; }
    }

    public class Thumbnail
    {
        public string? url { get; set; }
        public long? width { get; set; }
        public long? height { get; set; }
    }
}