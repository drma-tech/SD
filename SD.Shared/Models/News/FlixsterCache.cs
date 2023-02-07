using SD.Shared.Core.Models;

namespace SD.Shared.Models.News
{
    public class FlixsterCache : CacheDocument<Flixster>
    {
        public FlixsterCache()
        { }

        public FlixsterCache(Flixster data) : base("lastnews", data, ttlCache.one_day)
        { }
    }
}