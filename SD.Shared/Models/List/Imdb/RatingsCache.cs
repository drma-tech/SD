namespace SD.Shared.Models.List.Imdb
{
    public class RatingsCache : CacheModel<Ratings>
    {
        public RatingsCache()
        {
        }

        public RatingsCache(string key, Ratings data, ttlCache ttl)
        {
            Id = key;
            Key = key;
            Ttl = (int)ttl;
            Data = data;
        }
    }
}