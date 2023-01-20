namespace SD.Shared.Models.News
{
    public class FlixsterCache : CacheModel<Flixster>
    {
        public FlixsterCache()
        {
        }

        public FlixsterCache(string key, Flixster data, ttlCache ttl)
        {
            Id = key;
            Key = key;
            Ttl = (int)ttl;
            Data = data;
        }
    }
}