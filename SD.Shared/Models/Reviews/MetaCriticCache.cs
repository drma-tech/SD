using SD.Shared.Core.Models;

namespace SD.Shared.Models.Reviews
{
    public class MetaCriticCache : CacheDocument<ReviewModel>
    {
        public MetaCriticCache()
        { }

        public MetaCriticCache(ReviewModel data, string key, ttlCache ttl) : base(key, data, ttl)
        { }
    }
}