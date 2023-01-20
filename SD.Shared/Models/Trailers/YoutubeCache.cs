namespace SD.Shared.Models.Trailers
{
    public class YoutubeCache : CacheModel<Youtube>
    {
        public YoutubeCache()
        {
        }

        public YoutubeCache(string key, Youtube data, ttlCache ttl)
        {
            Id = key;
            Key = key;
            Ttl = (int)ttl;
            Data = data;
        }
    }
}