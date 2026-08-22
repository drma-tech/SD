using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SD.API.Core
{
    public static class CacheControl
    {
        public static async Task<T?> Get<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken)
            where T : class
        {
            var cachedBytes = await cache.GetAsync(key, cancellationToken);

            if (cachedBytes is { Length: > 0 })
            {
                return JsonSerializer.Deserialize<T>(cachedBytes);
            }
            else
            {
                return null;
            }
        }
    }
}