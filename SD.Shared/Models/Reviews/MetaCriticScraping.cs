namespace SD.Shared.Models.Reviews
{
    public class Data
    {
        public long? id { get; set; }
        public int totalResults { get; set; }
        public List<MetaCriticScrapingItem> items { get; set; } = [];
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

    public class MetaCriticScrapingItem
    {
        public string? quote { get; set; }
        public int score { get; set; }
        public string? url { get; set; }
        public string? date { get; set; }
        public string? author { get; set; }
        public string? authorSlug { get; set; }
        public object? image { get; set; }
        public string? publicationName { get; set; }
        public string? publicationSlug { get; set; }
        public ReviewedProduct? reviewedProduct { get; set; }
        public object? season { get; set; }
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
        public object? componentName { get; set; }
        public object? componentDisplayName { get; set; }
        public object? componentType { get; set; }
    }

    public class Next
    {
        public string? href { get; set; }
    }

    public class Prev
    {
        public string? href { get; set; }
    }

    public class ReviewedProduct
    {
        public long id { get; set; }
        public string? type { get; set; }
        public string? title { get; set; }
        public string? url { get; set; }
        public TvTaxonomy? tvTaxonomy { get; set; }
    }

    public class MetaCriticScraping
    {
        public Data? data { get; set; }
        public Links? links { get; set; }
        public Meta? meta { get; set; }
    }

    public class Season
    {
        public long id { get; set; }
        public string? name { get; set; }
    }

    public class Self
    {
        public string? href { get; set; }
    }

    public class SortOption
    {
        public string? label { get; set; }
        public string? value { get; set; }
        public string? href { get; set; }
    }

    public class TvTaxonomy
    {
        public Season? season { get; set; }
    }
}