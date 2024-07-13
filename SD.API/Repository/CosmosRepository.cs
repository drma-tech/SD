using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SD.API.Repository.Core;
using System.Linq.Expressions;

namespace SD.API.Repository
{
    public class CosmosRepository : IRepository
    {
        public Container Container { get; private set; }
        private readonly ILogger<CosmosRepository> _logger;

        public CosmosRepository(IConfiguration config, ILogger<CosmosRepository> logger)
        {
            _logger = logger;

            var databaseId = config.GetValue<string>("RepositoryOptions_DatabaseId");
            var containerId = config.GetValue<string>("RepositoryOptions_ContainerId");

            Container = ApiStartup.CosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task<T?> Get<T>(string id, PartitionKey key, CancellationToken cancellationToken) where T : CosmosDocument
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            try
            {
                var response = await Container.ReadItemAsync<T>(id, key, CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

                if (response.RequestCharge > 1.5)
                {
                    _logger.LogWarning("Get - ID {0}, RequestCharge {1}", id, response.RequestCharge);
                }

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<List<T>> ListAll<T>(DocumentType Type, CancellationToken cancellationToken) where T : MainDocument
        {
            var query = Container
                .GetItemLinqQueryable<T>(requestOptions: CosmosRepositoryExtensions.GetQueryRequestOptions(null))
                .Where(item => item.Type == Type);

            using var iterator = query.ToFeedIterator();
            var results = new List<T>();

            double charges = 0;
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                charges += response.RequestCharge;
                results.AddRange(response.Resource);
            }

            if (charges > 5)
            {
                _logger.LogWarning("ListAll - Type {Type}, RequestCharge {Charges}", Type.ToString(), charges);
            }

            return results;
        }

        public async Task<List<T>> Query<T>(Expression<Func<T, bool>> predicate, PartitionKey? key, DocumentType Type, CancellationToken cancellationToken) where T : MainDocument
        {
            var query = Container
                .GetItemLinqQueryable<T>(requestOptions: CosmosRepositoryExtensions.GetQueryRequestOptions(key))
                .Where(predicate.Compose(item => item.Type == Type, Expression.AndAlso));

            using var iterator = query.ToFeedIterator();
            var results = new List<T>();

            double charges = 0;
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                charges += response.RequestCharge;
                results.AddRange(response.Resource);
            }

            if (charges > 5)
            {
                _logger.LogWarning("Query - Type {Type}, RequestCharge {Charges}", Type.ToString(), charges);
            }

            return results;
        }

        public async Task<T> Upsert<T>(T item, CancellationToken cancellationToken) where T : CosmosDocument
        {
            var response = await Container.UpsertItemAsync(item, new PartitionKey(item.Id), CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

            if (response.RequestCharge > 8)
            {
                _logger.LogWarning("Upsert - ID {Id}, RequestCharge {Charges}", item.Id, response.RequestCharge);
            }

            return response.Resource;
        }

        public async Task<T> PatchItem<T>(string id, PartitionKey key, List<PatchOperation> operations, CancellationToken cancellationToken) where T : CosmosDocument
        {
            //https://learn.microsoft.com/en-us/azure/cosmos-db/partial-document-update-getting-started?tabs=dotnet

            var response = await Container.PatchItemAsync<T>(id, key, operations, CosmosRepositoryExtensions.GetPatchItemRequestOptions(), cancellationToken);

            if (response.RequestCharge > 8)
            {
                _logger.LogWarning("PatchItem - ID {Id}, RequestCharge {Charges}", id, response.RequestCharge);
            }

            return response.Resource;
        }

        public async Task<bool> Delete<T>(T item, CancellationToken cancellationToken) where T : CosmosDocument
        {
            var response = await Container.DeleteItemAsync<T>(item.Id, new PartitionKey(item.Id), CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

            if (response.RequestCharge > 8)
            {
                _logger.LogWarning("Delete - ID {Id}, RequestCharge {Charges}", item.Id, response.RequestCharge);
            }

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        //multiple transactions
        //https://docs.microsoft.com/pt-br/learn/modules/perform-cross-document-transactional-operations-azure-cosmos-db-sql-api/2-create-transactional-batch-sdk

        //bulk insert
        //https://docs.microsoft.com/pt-br/learn/modules/process-bulk-data-azure-cosmos-db-sql-api/2-create-bulk-operations-sdk

        //composite indexes
        //https://docs.microsoft.com/pt-br/learn/modules/customize-indexes-azure-cosmos-db-sql-api/3-evaluate-composite-indexes
    }
}