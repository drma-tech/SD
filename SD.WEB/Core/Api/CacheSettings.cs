using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Core.Api
{
    public class CacheSettings : MemoryCacheEntryOptions
    {
        public CacheSettings(TimeSpan? SlidingExpiration = null, TimeSpan? AbsoluteExpirationRelativeToNow = null)
        {
            this.SlidingExpiration = SlidingExpiration ?? TimeSpan.FromMinutes(5);
            this.AbsoluteExpirationRelativeToNow = AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(10);
        }
    }
}
