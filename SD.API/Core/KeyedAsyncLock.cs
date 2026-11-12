using Microsoft.Azure.Cosmos;
using System.Collections.Concurrent;
using System.Net;

namespace SD.API.Core
{
    public static class KeyedAsyncLock
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new(StringComparer.OrdinalIgnoreCase);

        public static async Task<T?> GetOrCreateAsync<T>(string key,
            Func<CancellationToken, Task<T?>> getCache,
            Func<CancellationToken, Task<T?>> getPersistent,
            Func<CancellationToken, Task<T?>> create,
            Func<T?, CancellationToken, Task> saveCache,
            CancellationToken cancellationToken) where T : class
        {
            var value = await getCache(cancellationToken);

            if (value != null) return value;

            var semaphore = Locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync(cancellationToken);

            try
            {
                value = await getCache(cancellationToken);

                if (value != null) return value;

                value = await getPersistent(cancellationToken);

                if (value != null)
                {
                    await saveCache(value, cancellationToken);
                    return value;
                }

                try
                {
                    value = await create(cancellationToken);
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    // Azure Functions can run on multiple instances, so the local lock does not prevent another instance from creating the same item.

                    for (int attempt = 0; attempt < 3; attempt++)
                    {
                        await Task.Delay(50 * (attempt + 1), cancellationToken); // 50, 100, 150ms

                        value = await getPersistent(cancellationToken);
                        if (value != null)
                        {
                            await saveCache(value, cancellationToken);
                            return value;
                        }
                    }

                    throw;
                }

                await saveCache(value, cancellationToken);

                return value;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
