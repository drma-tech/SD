using SD.Shared.Core.Models;

namespace SD.Shared.Models.Trailers
{
    public class YoutubeCache : CacheDocument<Youtube>
    {
        public YoutubeCache()
        { }

        public YoutubeCache(Youtube data) : base("lasttrailers", data, ttlCache.one_day)
        { }
    }
}