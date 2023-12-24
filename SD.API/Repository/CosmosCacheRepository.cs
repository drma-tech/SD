using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SD.Shared.Core.Models;
using SD.Shared.Models.List.Tmdb;

namespace SD.API.Repository
{
    public class CacheSettings : MemoryCacheEntryOptions
    {
        public CacheSettings(TimeSpan? SlidingExpiration = null, TimeSpan? AbsoluteExpirationRelativeToNow = null)
        {
            this.SlidingExpiration = SlidingExpiration ?? TimeSpan.FromHours(24);
            this.AbsoluteExpirationRelativeToNow = AbsoluteExpirationRelativeToNow ?? TimeSpan.FromHours(24);
        }
    }

    public class CosmosCacheRepository
    {
        public CacheSettings? CacheSettings { get; set; } = new CacheSettings();
        public Container Container { get; private set; }
        protected IMemoryCache _cache { get; set; }

        public CosmosCacheRepository(IConfiguration config, IMemoryCache memoryCache)
        {
            var connString = config.GetValue<string>("RepositoryOptions_CosmosConnectionString");
            var databaseId = config.GetValue<string>("RepositoryOptions_DatabaseId");
            var containerId = config.GetValue<string>("RepositoryOptions_ContainerCacheId");

            var _client = new CosmosClient(connString, new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });

            Container = _client.GetContainer(databaseId, containerId);
            _cache = memoryCache;
        }

        public async Task<CacheDocument<TData>?> Get<TData>(string key, CancellationToken cancellationToken) where TData : class
        {
            var response = _cache.Get<ItemResponse<CacheDocument<TData>?>>(key);

            try
            {
                if (response == null)
                {
                    response = await Container.ReadItemAsync<CacheDocument<TData>?>(key, new PartitionKey(key), null, cancellationToken);

                    _cache.Set(key, response, CacheSettings);
                }

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<CacheDocument<TData>?> Add<TData>(CacheDocument<TData> cache, CancellationToken cancellationToken) where TData : class
        {
            var response = await Container.UpsertItemAsync(cache, new PartitionKey(cache.Key), null, cancellationToken);

            _cache.Set(cache.Key, response, CacheSettings);

            return response.Resource;
        }
    }
}