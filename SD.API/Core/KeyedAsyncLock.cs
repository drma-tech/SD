using System.Collections.Concurrent;

namespace SD.API.Core
{
    public static class KeyedAsyncLock
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new(StringComparer.OrdinalIgnoreCase);

        public static async Task<T> GetOrCreateAsync<T>(string key,
            Func<CancellationToken, Task<T?>> getCache,
            Func<CancellationToken, Task<T?>> getPersistent,
            Func<CancellationToken, Task<T?>> create,
            Func<T, CancellationToken, Task> saveCache,
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

                value = await create(cancellationToken);

                //todo: try this if its still happening
                //try
                //{
                //    return await create(cancellationToken);
                //}
                //catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                //{
                //    value = await get(cancellationToken);
                //    if (value != null) return value;
                //    throw;
                //}

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