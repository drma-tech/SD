namespace SD.Shared.Models.Reviews.MetaCriticSite
{
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

    public class Item
    {
        public string? id { get; set; }
        public ReviewedProduct? reviewedProduct { get; set; }
        public string? reviewPath { get; set; }
        public string? quote { get; set; }
        public string? platform { get; set; }
        public int? score { get; set; }
        public int? metaScore { get; set; }
        public string? url { get; set; }
        public string? date { get; set; }
        public string? author { get; set; }
        public string? authorSlug { get; set; }
        public string? publicationName { get; set; }
        public string? publicationSlug { get; set; }
        public string? image { get; set; }
        public int? season { get; set; }
        public int? episode { get; set; }
        public int? thumbsUp { get; set; }
        public int? thumbsDown { get; set; }
        public bool? spoiler { get; set; }
        public int? version { get; set; }
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
    }

    public class Next
    {
        public object? href { get; set; }
    }

    public class Prev
    {
        public object? href { get; set; }
    }

    public class ReviewedProduct
    {
        public object? id { get; set; }
        public string? type { get; set; }
        public string? title { get; set; }
        public string? url { get; set; }
        public TvTaxonomy? tvTaxonomy { get; set; }
    }

    public class RootMetacriticReview
    {
        public string? id { get; set; }
        public Meta? meta { get; set; }
        public object? metadata { get; set; }
        public Links? links { get; set; }
        public List<Item> items { get; set; } = [];
        public int total { get; set; }
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