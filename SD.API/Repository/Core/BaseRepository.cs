using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net;

namespace SD.API.Repository.Core
{
    public abstract class BaseRepository<TClass, TData, TId>
        where TClass : class
        where TData : CosmosDocument
        where TId : ICosmosIdentity
    {
        protected readonly ILogger<TClass> _logger;
        protected Container Container { get; }
        private const double extra = 1;

        protected BaseRepository(CosmosClient CosmosClient, ILogger<TClass> logger, string containerId)
        {
            _logger = logger;

            var databaseId = ApiStartup.Configurations.CosmosDB?.DatabaseId;

            Container = CosmosClient.GetContainer(databaseId, containerId);
        }

        public virtual async Task<T?> ReadItemAsync<T>(TId id, CancellationToken cancellationToken) where T : TData
        {
            try
            {
                var response = await Container.ReadItemAsync<T?>(id.Id, id.Key.ToPartitionKey(), null, cancellationToken);

                if (response.RequestCharge > 1d + extra) //weight: 1
                    _logger.LogWarning("ReadItemAsync - Id {Id}, RequestCharge {RequestCharge}", id, response.RequestCharge);

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

        public virtual async Task<T> CreateItemAsync<T>(T item) where T : TData
        {
            try
            {
                var response = await Container.CreateItemAsync(item, item.Identity.Key.ToPartitionKey(), null);

                if (response.RequestCharge > 6d + extra) //weight: 6
                    _logger.LogWarning("CreateItemAsync - ID {Id}, RequestCharge {Charges}", item.Identity.Id, response.RequestCharge);

                return response.Resource;
            }
            catch (CosmosOperationCanceledException)
            {
                return item;
            }
        }

        public virtual async Task<T> UpsertItemAsync<T>(T item) where T : TData
        {
            try
            {
                var response = await Container.UpsertItemAsync(item, item.Identity.Key.ToPartitionKey(), null);

                if (response.RequestCharge > 10d + extra) //weight: 10 (6 if new doc)
                    _logger.LogWarning("UpsertItemAsync - ID {Id}, RequestCharge {Charges}", item.Identity.Id, response.RequestCharge);

                return response.Resource;
            }
            catch (CosmosOperationCanceledException)
            {
                return item;
            }
        }

        public virtual async Task<bool> DeleteItemAsync<T>(TId id) where T : TData
        {
            try
            {
                var response = await Container.DeleteItemAsync<T>(id.Id, id.Key.ToPartitionKey(), null);

                if (response.RequestCharge > 6d + extra) //weight: 6
                    _logger.LogWarning("DeleteItemAsync - ID {Id}, RequestCharge {Charges}", id.Id, response.RequestCharge);

                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return true;
            }
            catch (CosmosOperationCanceledException)
            {
                return false;
            }
        }
    }
}
