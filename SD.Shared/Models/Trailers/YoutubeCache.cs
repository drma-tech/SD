namespace SD.Shared.Models.Trailers
{
    public class YoutubeCache : CacheDocument<TrailerModel>
    {
        public YoutubeCache()
        { }

        public YoutubeCache(TrailerModel data, string key) : base(key, data, ttlCache.half_day)
        { }
    }
}