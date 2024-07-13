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

        public async Task<CacheDocument<TData>?> Get<TData>(string id, CancellationToken cancellationToken) where TData : class
        {
            try
            {
                var response = await Container.ReadItemAsync<CacheDocument<TData>?>(id, new PartitionKey(id), CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

                if (response.RequestCharge > 1.5)
                {
                    _logger.LogWarning("Get - Id {0}, RequestCharge {1}", id, response.RequestCharge);
                }

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<CacheDocument<TData>?> CreateItemAsync<TData>(CacheDocument<TData> cache, CancellationToken cancellationToken) where TData : class
        {
            var response = await Container.CreateItemAsync(cache, new PartitionKey(cache.Id), CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

            if (response.RequestCharge > 12)
            {
                _logger.LogWarning("Add - Id {0}, RequestCharge {1}", cache.Id, response.RequestCharge);
            }

            return response.Resource;
        }
    }
}