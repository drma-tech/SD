using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using SD.Shared.Core.Models;

namespace SD.API.Repository
{
    public class CosmosCacheRepository
    {
        public Container Container { get; private set; }

        public CosmosCacheRepository(IConfiguration config)
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
        }

        public async Task<CacheDocument<TData>?> Get<TData>(string key, CancellationToken cancellationToken) where TData : class
        {
            try
            {
                var response = await Container.ReadItemAsync<CacheDocument<TData>>(key, new PartitionKey(key), null, cancellationToken);

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

            return response.Resource;
        }
    }
}