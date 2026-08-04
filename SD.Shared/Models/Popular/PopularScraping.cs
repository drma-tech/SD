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
        public Thumbnail[]? thumbnails { get; set; }
        public string? trailer { get; set; }
        public string? contentRating { get; set; }
        public long? startYear { get; set; }
        public long? endYear { get; set; }
        public string? releaseDate { get; set; }
        public IReadOnlyCollection<string>? interests { get; set; }
        public IReadOnlyCollection<string>? countriesOfOrigin { get; set; }
        public IReadOnlyCollection<string>? externalLinks { get; set; }
        public IReadOnlyCollection<string>? spokenLanguages { get; set; }
        public IReadOnlyCollection<string>? filmingLocations { get; set; }
        public IReadOnlyCollection<ProductionCompany>? productionCompanies { get; set; }
        public long? budget { get; set; }
        public long? grossWorldwide { get; set; }
        public IReadOnlyCollection<string>? genres { get; set; }
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