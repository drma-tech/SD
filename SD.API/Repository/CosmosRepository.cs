using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using SD.API.Repository.Core;
using System.Linq.Expressions;

namespace SD.API.Repository
{
    public class CosmosRepository : IRepository
    {
        public Container Container { get; private set; }

        public CosmosRepository(IConfiguration config)
        {
            var connString = config.GetValue<string>("RepositoryOptions_CosmosConnectionString");
            var databaseId = config.GetValue<string>("RepositoryOptions_DatabaseId");
            var containerId = config.GetValue<string>("RepositoryOptions_ContainerId");

            var _client = new CosmosClient(connString, new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });

            Container = _client.GetContainer(databaseId, containerId);

            //Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");

            //Container container = await database.CreateContainerIfNotExistsAsync(
            //    "cosmicworks",
            //    "/categoryId",
            //    400
            //);

            //IndexingPolicy policy = new()
            //{
            //    IndexingMode = IndexingMode.Consistent,
            //    Automatic = true
            //};
            //policy.ExcludedPaths.Add(
            //    new ExcludedPath { Path = "/name/?" }
            //);

            //ContainerProperties options = new()
            //{
            //    Id = "products",
            //    PartitionKeyPath = "/categoryId",
            //    IndexingPolicy = policy
            //};
            //Container container = await database.CreateContainerIfNotExistsAsync(options, throughput: 400);
        }

        public async Task<T?> Get<T>(string id, PartitionKey key, CancellationToken cancellationToken) where T : CosmosDocument
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            try
            {
                var response = await Container.ReadItemAsync<T>(id, key, null, cancellationToken);

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<List<T>> Query<T>(Expression<Func<T, bool>>? predicate, PartitionKey? key, DocumentType Type, CancellationToken cancellationToken) where T : MainDocument
        {
            IQueryable<T> query;

            if (predicate is null)
            {
                query = Container.GetItemLinqQueryable<T>(requestOptions: CosmosRepositoryExtensions.GetDefaultOptions(key))
                    .Where(item => item.Type == Type);
            }
            else
            {
                query = Container.GetItemLinqQueryable<T>(requestOptions: CosmosRepositoryExtensions.GetDefaultOptions(key))
                    .Where(predicate.Compose(item => item.Type == Type, Expression.AndAlso));
            }

            using var iterator = query.ToFeedIterator();
            var results = new List<T>();
            double count = 0;

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);

                count += response.RequestCharge;

                results.AddRange(response.Resource);
            }

            return results;
        }

        public async Task<List<T>> Query<T>(QueryDefinition query, CancellationToken cancellationToken) where T : MainDocument
        {
            using var iterator = Container.GetItemQueryIterator<T>(query);
            var results = new List<T>();
            double count = 0;

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);

                count += response.RequestCharge;

                results.AddRange(response.Resource);
            }

            return results;
        }

        public async Task<T> Upsert<T>(T item, CancellationToken cancellationToken) where T : CosmosDocument
        {
            var response = await Container.UpsertItemAsync(item, new PartitionKey(item.Key), null, cancellationToken);

            return response.Resource;
        }

        public async Task<T> PatchItem<T>(string id, PartitionKey key, List<PatchOperation> operations, CancellationToken cancellationToken) where T : CosmosDocument
        {
            //https://learn.microsoft.com/en-us/azure/cosmos-db/partial-document-update-getting-started?tabs=dotnet

            var response = await Container.PatchItemAsync<T>(id, key, operations, null, cancellationToken);

            return response.Resource;
        }

        public async Task<bool> Delete<T>(T item, CancellationToken cancellationToken) where T : CosmosDocument
        {
            var response = await Container.DeleteItemAsync<T>(item.Id, new PartitionKey(item.Key), null, cancellationToken);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        //multiple transactions
        //https://docs.microsoft.com/pt-br/learn/modules/perform-cross-document-transactional-operations-azure-cosmos-db-sql-api/2-create-transactional-batch-sdk

        //bulk insert
        //https://docs.microsoft.com/pt-br/learn/modules/process-bulk-data-azure-cosmos-db-sql-api/2-create-bulk-operations-sdk

        //composite indexes
        //https://docs.microsoft.com/pt-br/learn/modules/customize-indexes-azure-cosmos-db-sql-api/3-evaluate-composite-indexes
    }

    public static class CosmosRepositoryExtensions
    {
        public static QueryRequestOptions? GetDefaultOptions(PartitionKey? key)
        {
            if (key == null)
                return null;
            else
                return new QueryRequestOptions()
                {
                    PartitionKey = key,
                    //https://learn.microsoft.com/en-us/training/modules/measure-index-azure-cosmos-db-sql-api/4-measure-query-cost
                    MaxItemCount = 10, //max itens per page
                    //https://learn.microsoft.com/en-us/training/modules/measure-index-azure-cosmos-db-sql-api/2-enable-indexing-metrics
                    PopulateIndexMetrics = false //enable only when analysing metrics
                };
        }
    }
}