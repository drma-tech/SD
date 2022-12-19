namespace SD.Shared.Models.List.Imdb
{
    public class RatingsCache : CacheModel<Ratings>
    {
        public RatingsCache()
        {
        }

        public RatingsCache(string key, Ratings data, int? ttl = one_month)
        {
            ttl ??= one_month;

            Id = key;
            Key = key;
            Ttl = ttl.Value;
            Data = data;
        }
    }
}