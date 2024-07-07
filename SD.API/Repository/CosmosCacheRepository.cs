using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SD.API.Repository.Core;

namespace SD.API.Repository
{
    public class CosmosCacheRepository
    {
        public Container Container { get; private set; }
        private readonly ILogger<CosmosCacheRepository> _logger;

        public CosmosCacheRepository(IConfiguration config, ILogger<CosmosCacheRepository> logger)
        {
            _logger = logger;

            var databaseId = config.GetValue<string>("RepositoryOptions_DatabaseId");
            var containerId = config.GetValue<string>("RepositoryOptions_ContainerCacheId");

            Container = ApiStartup.CosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task<CacheDocument<TData>?> Get<TData>(string key, CancellationToken cancellationToken) where TData : class
        {
            try
            {
                var response = await Container.ReadItemAsync<CacheDocument<TData>?>(key, new PartitionKey(key), CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

                if (response.RequestCharge > 1.7)
                {
                    _logger.LogWarning("Get - key {0}, RequestCharge {1}", key, response.RequestCharge);
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
            var response = await Container.UpsertItemAsync(cache, new PartitionKey(cache.Key), CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

            if (response.RequestCharge > 12)
            {
                _logger.LogWarning("Add - Key {0}, RequestCharge {1}", cache.Key, response.RequestCharge);
            }

            return response.Resource;
        }
    }
}