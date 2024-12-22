namespace SD.Shared.Models.News
{
    public class FlixsterCache : CacheDocument<NewsModel>
    {
        public FlixsterCache()
        { }

        public FlixsterCache(NewsModel data, string key) : base(key, data, ttlCache.half_day)
        { }
    }
}