namespace SD.Shared.Models.Reviews;

public class MetaCriticCache(string id, ReviewModel data, TtlCache ttl) : CacheDocumentData<ReviewModel>(new CacheIdentity(id), data, ttl)
{
}

public class ReviewModel
{
    public List<ReviewModelItem> Items { get; set; } = [];
}

public class ReviewModelItem
{
    public ReviewModelItem()
    {
    }

    public ReviewModelItem(string? reviewSite, string? reviewUrl, string? reviewer, int? score, string? quote)
    {
        this.reviewSite = reviewSite;
        this.reviewUrl = reviewUrl;
        this.reviewer = reviewer;
        this.score = score;
        this.quote = quote;
    }

    public string? reviewSite { get; set; }
    public string? reviewUrl { get; set; }
    public string? reviewer { get; set; }
    public int? score { get; set; }
    public string? quote { get; set; }
}