namespace SD.Shared.Models.Reviews
{
    public class Award
    {
        public string? awardEvent { get; set; }
        public int? wins { get; set; }
        public int? nominations { get; set; }
    }

    public class Cast
    {
        public object? id { get; set; }
        public string? name { get; set; }
        public List<string> roles { get; set; } = [];
        public int? roleTypeGroupId { get; set; }
        public string? roleTypeGroupName { get; set; }
        public EntertainmentProduct? entertainmentProduct { get; set; }
        public Image? image { get; set; }
    }

    public class Company
    {
        public object? id { get; set; }
        public int? typeId { get; set; }
        public string? typeName { get; set; }
        public string? name { get; set; }
        public string? url { get; set; }
    }

    public class Component
    {
        public Data? data { get; set; }
        public Links? links { get; set; }
        public Meta? meta { get; set; }
    }

    public class Crew
    {
        public object? id { get; set; }
        public string? name { get; set; }
        public List<string> roles { get; set; } = [];
        public int? roleTypeGroupId { get; set; }
        public string? roleTypeGroupName { get; set; }
        public EntertainmentProduct? entertainmentProduct { get; set; }
        public Image? image { get; set; }
    }

    public class CriticScoreSummary
    {
        public string? url { get; set; }
        public int? max { get; set; }
        public int? score { get; set; }
        public double? normalizedScore { get; set; }
        public int? reviewCount { get; set; }
        public int? positiveCount { get; set; }
        public int? neutralCount { get; set; }
        public int? negativeCount { get; set; }
        public string? sentiment { get; set; }
    }

    public class Data
    {
        public Item1? item { get; set; }
        public object? id { get; set; }
        public int? totalResults { get; set; }
        public List<Item2> items { get; set; } = [];
        public string? title { get; set; }
        public string? componentLabel { get; set; }
        public string? root { get; set; }
    }

    public class DateCreated
    {
        public object? date { get; set; }
        public object? timezone { get; set; }
    }

    public class EntertainmentProduct
    {
        public object? id { get; set; }
        public string? name { get; set; }
        public string? slug { get; set; }
        public int? typeId { get; set; }
        public string? url { get; set; }
        public List<object> profession { get; set; } = [];
    }

    public class FilterOption
    {
        public string? label { get; set; }
        public string? value { get; set; }
        public string? href { get; set; }
    }

    public class First
    {
        public string? href { get; set; }
    }

    public class Genre
    {
        public object? id { get; set; }
        public string? name { get; set; }
    }

    public class Header
    {
        public Data? data { get; set; }
        public Links? links { get; set; }
        public Meta? meta { get; set; }
    }

    public class Image
    {
        public string? id { get; set; }
        public string? filename { get; set; }
        public DateCreated? dateCreated { get; set; }
        public object? alt { get; set; }
        public object? credits { get; set; }
        public object? path { get; set; }
        public object? cropGravity { get; set; }
        public object? crop { get; set; }
        public object? caption { get; set; }
        public string? typeName { get; set; }
        public object? imageUrl { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public object? sType { get; set; }
        public string? bucketType { get; set; }
        public string? bucketPath { get; set; }
        public object? mediaType { get; set; }
        public string? provider { get; set; }
    }

    public class Image2
    {
        public string? id { get; set; }
        public string? filename { get; set; }
        public DateCreated? dateCreated { get; set; }
        public object? alt { get; set; }
        public object? credits { get; set; }
        public object? path { get; set; }
        public object? cropGravity { get; set; }
        public object? crop { get; set; }
        public object? caption { get; set; }
        public string? typeName { get; set; }
        public object? imageUrl { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public object? sType { get; set; }
        public string? bucketType { get; set; }
        public string? bucketPath { get; set; }
        public object? mediaType { get; set; }
        public string? provider { get; set; }
    }

    public class Item1
    {
        public int? id { get; set; }
        public string? type { get; set; }
        public int? typeId { get; set; }
        public string? title { get; set; }
        public string? slug { get; set; }
        public int? premiereYear { get; set; }
        public string? releaseDate { get; set; }
        public string? description { get; set; }
        public List<Image> images { get; set; } = [];
        public Image? image { get; set; }
        public Production? production { get; set; }
        public CriticScoreSummary? criticScoreSummary { get; set; }
        public List<string> tags { get; set; } = [];
        public List<Genre> genres { get; set; } = [];
        public string? rating { get; set; }
        public int? duration { get; set; }
        public List<Award> awards { get; set; } = [];
        public int? seasonCount { get; set; }
        public string? tagline { get; set; }
        public TvTaxonomy? tvTaxonomy { get; set; }
        public List<Network> networks { get; set; } = [];
        public string? imdbLink { get; set; }
        public Video? video { get; set; }
        public int? max { get; set; }
        public int? score { get; set; }
        public int? reviewCount { get; set; }
        public int? positiveCount { get; set; }
        public int? neutralCount { get; set; }
        public int? negativeCount { get; set; }
        public string? sentiment { get; set; }
        public string? url { get; set; }
    }

    public class Item2
    {
        public string? quote { get; set; }
        public int? score { get; set; }
        public string? url { get; set; }
        public string? date { get; set; }
        public string? author { get; set; }
        public string? authorSlug { get; set; }
        public object? image { get; set; }
        public string? publicationName { get; set; }
        public string? publicationSlug { get; set; }
        public ReviewedProduct? reviewedProduct { get; set; }
        public object? season { get; set; }
        public long? id { get; set; }
        public string? title { get; set; }
        public int? releaseYear { get; set; }
        public string? premiereDate { get; set; }
        public object? description { get; set; }
        public CriticScoreSummary? criticScoreSummary { get; set; }
        public int? seasonNumber { get; set; }
        public int? totalEpisodes { get; set; }
        public string? type { get; set; }
        public List<object> episodes { get; set; } = [];
        public TvTaxonomy? tvTaxonomy { get; set; }
        public object? imdbLink { get; set; }
        public string? link { get; set; }
    }

    public class Last
    {
        public string? href { get; set; }
    }

    public class Links
    {
        public Self? self { get; set; }
        public Prev? prev { get; set; }
        public Next? next { get; set; }
        public First? first { get; set; }
        public Last? last { get; set; }
        public List<FilterOption> filterOptions { get; set; } = [];
        public List<SortOption> sortOptions { get; set; } = [];
    }

    public class Meta
    {
        public string? componentName { get; set; }
        public string? componentDisplayName { get; set; }
        public string? componentType { get; set; }
        public string? edition { get; set; }
        public string? title { get; set; }
        public string? id { get; set; }
        public string? slug { get; set; }
        public object? category { get; set; }
        public object? description { get; set; }
        public string? pageType { get; set; }
        public object? section { get; set; }
        public Image? image { get; set; }
        public string? typeName { get; set; }
        public List<string> genres { get; set; } = [];
        public List<string> themes { get; set; } = [];
        public List<string> publishers { get; set; } = [];
    }

    public class Network
    {
        public long id { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
        public string? url { get; set; }
    }

    public class Next
    {
        public object? href { get; set; }
    }

    public class Prev
    {
        public object? href { get; set; }
    }

    public class Production
    {
        public List<Company> companies { get; set; } = [];
        public object? officialSiteUrl { get; set; }
        public List<Cast> cast { get; set; } = [];
        public List<Crew> crew { get; set; } = [];
    }

    public class ReviewedProduct
    {
        public object? id { get; set; }
        public string? type { get; set; }
        public string? title { get; set; }
        public string? url { get; set; }
        public TvTaxonomy? tvTaxonomy { get; set; }
    }

    public class MetaCriticScraping
    {
        public List<Component> components { get; set; } = [];
        public Meta? meta { get; set; }
        public List<Header> header { get; set; } = [];
    }

    public class Season
    {
        public object? id { get; set; }
        public string? name { get; set; }
    }

    public class Self
    {
        public string? href { get; set; }
    }

    public class Show
    {
        public int? id { get; set; }
        public string? name { get; set; }
    }

    public class SortOption
    {
        public string? label { get; set; }
        public string? value { get; set; }
        public string? href { get; set; }
    }

    public class TvTaxonomy
    {
        public Show? show { get; set; }
        public Season? season { get; set; }
    }

    public class Video
    {
        public int? videoId { get; set; }
        public int? providerId { get; set; }
        public string? title { get; set; }
        public string? slug { get; set; }
        public string? type { get; set; }
        public string? contentType { get; set; }
        public int? contentTypeId { get; set; }
        public string? videoTitle { get; set; }
        public string? url { get; set; }
        public object? description { get; set; }
        public int? duration { get; set; }
        public DateTime originalAirDate { get; set; }
        public int? seasonNumber { get; set; }
        public int? episodeNumber { get; set; }
        public List<Image> images { get; set; } = [];
        public string? jwPlayerId { get; set; }
    }
}