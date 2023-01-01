namespace SD.Shared.Models.Trailers
{
    public class YoutubeCache : CacheModel<Youtube>
    {
        public YoutubeCache()
        {
        }

        public YoutubeCache(string key, Youtube data, int? ttl = one_day)
        {
            ttl ??= one_day;

            Id = key;
            Key = key;
            Ttl = ttl.Value;
            Data = data;
        }
    }
}