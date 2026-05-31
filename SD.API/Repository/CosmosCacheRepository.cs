using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net;

namespace SD.API.Repository;

public class CosmosCacheRepository
{
    private readonly ILogger<CosmosCacheRepository> _logger;

    public CosmosCacheRepository(CosmosClient CosmosClient, ILogger<CosmosCacheRepository> logger)
    {
        _logger = logger;

        var databaseId = ApiStartup.Configurations.CosmosDB?.DatabaseId;

        Container = CosmosClient.GetContainer(databaseId, "cache");
    }

    public Container Container { get; }

    public async Task<CacheDocument<TData>?> Get<TData>(string id, CancellationToken cancellationToken) where TData : class, new()
    {
        //var sw = Stopwatch.StartNew();

        try
        {
            var response = await Container.ReadItemAsync<CacheDocument<TData>?>(id, new PartitionKey(id), null, cancellationToken);

            //sw.Stop();

            //_logger.LogWarning("Cosmos ReadItemAsync finished. Id={Id}, DurationMs={DurationMs}, RU={RU}, StatusCode={StatusCode}", id, sw.ElapsedMilliseconds, response.RequestCharge, response.StatusCode);
            //_logger.LogWarning("Cosmos Diagnostics. Id={Id}, Diagnostics={Diagnostics}", id, response.Diagnostics);

            if (response.RequestCharge > 3d)
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

    public async Task<CacheDocument<TData>?> CreateItemAsync<TData>(CacheDocument<TData> cache, CancellationToken cancellationToken) where TData : class, new()
    {
        try
        {
            var response = await Container.CreateItemAsync(cache, new PartitionKey(cache.Id), null, cancellationToken);

            if (response.RequestCharge > 15d)
                _logger.LogWarning("CreateItemAsync - Id {Id}, RequestCharge {RequestCharge}", cache.Id, response.RequestCharge);

            return response.Resource;
        }
        catch (CosmosOperationCanceledException)
        {
            return null;
        }
    }

    public async Task<CacheDocument<TData>?> UpsertItemAsync<TData>(CacheDocument<TData> cache, CancellationToken cancellationToken) where TData : class, new()
    {
        try
        {
            var response = await Container.UpsertItemAsync(cache, new PartitionKey(cache.Id), null, cancellationToken);

            if (response.RequestCharge > 15d)
                _logger.LogWarning("UpsertItemAsync - Id {Id}, RequestCharge {RequestCharge}", cache.Id, response.RequestCharge);

            return response.Resource;
        }
        catch (CosmosOperationCanceledException)
        {
            return null;
        }
    }
}