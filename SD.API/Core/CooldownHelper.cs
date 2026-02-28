using Microsoft.Extensions.Caching.Distributed;

namespace SD.API.Core
{
    public class ApiRateLimitException(string message) : Exception(message)
    {
    }

    public static class CooldownHelper
    {
        public static async Task ExecuteWithCooldownAsync(this IDistributedCache cache, string providerName, Func<Task> action, CancellationToken cancellationToken)
        {
            var cooldownKey = $"cooldown_{providerName}";

            var isCoolingDown = await cache.GetAsync(cooldownKey, cancellationToken);

            if (isCoolingDown is not null)
                return;

            try
            {
                await action();
            }
            catch (ApiRateLimitException)
            {
                await cache.SetAsync(cooldownKey, [1], new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }, cancellationToken);
            }
        }
    }
}