using Microsoft.IdentityModel.Tokens;

namespace SD.API.Core
{
    public static class JwksCache
    {
        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        private static List<SecurityKey>? _cachedKeys;
        private static DateTime _lastFetch = DateTime.MinValue;
        private static bool _forceRefresh = false;

        public static TimeSpan MinRefreshInterval { get; set; } = TimeSpan.FromMinutes(1);
        public static TimeSpan MaxCacheDuration { get; set; } = TimeSpan.FromHours(6);

        public static async Task<IReadOnlyList<SecurityKey>> GetKeysAsync(string jwksUri, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            // Cache valid if: we have keys, it is not forcing refresh, and within the maximum time
            if (_cachedKeys != null && !_forceRefresh && now - _lastFetch < MaxCacheDuration)
                return _cachedKeys;

            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                // Double check after lock
                if (_cachedKeys != null && !_forceRefresh && now - _lastFetch < MaxCacheDuration)
                    return _cachedKeys;

                // Respects minimum time since last fetch (success or error)
                if (now - _lastFetch < MinRefreshInterval)
                {
                    // Minimum time not reached - returns current cache even if forceRefresh
                    if (_cachedKeys != null)
                        return _cachedKeys;

                    throw new InvalidOperationException("No cached keys available");
                }

                // Fetch the new keys
                using var http = new HttpClient();
                var json = await http.GetStringAsync(jwksUri, cancellationToken);
                var keys = new JsonWebKeySet(json).GetSigningKeys().ToList();

                _cachedKeys = keys;
                _lastFetch = DateTime.UtcNow;
                _forceRefresh = false; // Resets the force refresh flag

                return _cachedKeys;
            }
            catch (Exception)
            {
                _lastFetch = DateTime.UtcNow; // Mark attempt (successful or not)
                if (_cachedKeys != null)
                    return _cachedKeys; // Fallback to old cache
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static void Invalidate()
        {
            _forceRefresh = true; // Mark that you need a refresh on the next call
        }
    }
}