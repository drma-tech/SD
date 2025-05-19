namespace SD.Shared.Models.Reviews;

public class MetaCriticCache : CacheDocument<ReviewModel>
{
    public MetaCriticCache()
    {
    }

    public MetaCriticCache(ReviewModel data, string key, TtlCache ttl) : base(key, data, ttl)
    {
    }
}