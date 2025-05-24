using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using SD.API.Repository.Core;
using System.Net;

namespace SD.API.Repository;

public class CosmosCacheRepository
{
    private readonly ILogger<CosmosCacheRepository> _logger;

    public CosmosCacheRepository(ILogger<CosmosCacheRepository> logger)
    {
        _logger = logger;

        var databaseId = ApiStartup.Configurations.CosmosDB?.DatabaseId;

        Container = ApiStartup.CosmosClient.GetContainer(databaseId, "cache");
    }

    public Container Container { get; }

    public async Task<CacheDocument<TData>?> Get<TData>(string id, CancellationToken cancellationToken)
        where TData : class, new()
    {
        try
        {
            var response = await Container.ReadItemAsync<CacheDocument<TData>?>(id, new PartitionKey(id),
                CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

            if (response.RequestCharge > 1.7)
                _logger.LogWarning("Get - Id {Id}, RequestCharge {RequestCharge}", id, response.RequestCharge);

            return response.Resource;
        }
        catch (CosmosOperationCanceledException)
        {
            return null;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<CacheDocument<TData>?> UpsertItemAsync<TData>(CacheDocument<TData> cache,
        CancellationToken cancellationToken) where TData : class, new()
    {
        try
        {
            var response = await Container.UpsertItemAsync(cache, new PartitionKey(cache.Id),
                CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

            if (response.RequestCharge > 15)
                _logger.LogWarning("Add - Id {Id}, RequestCharge {RequestCharge}", cache.Id, response.RequestCharge);

            return response.Resource;
        }
        catch (CosmosOperationCanceledException)
        {
            return null;
        }
    }
}