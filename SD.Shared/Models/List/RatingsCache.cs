namespace SD.Shared.Models.List;

public class RatingsCache : CacheDocument<Ratings>
{
    public RatingsCache()
    {
    }

    public RatingsCache(string? id, Ratings data, ttlCache ttl) : base($"rating_{id}", data, ttl)
    {
    }
}