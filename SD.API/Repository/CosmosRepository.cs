﻿using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SD.API.Repository.Core;
using System.Linq.Expressions;

namespace SD.API.Repository
{
    public class CosmosRepository
    {
        public Container Container { get; private set; }
        private readonly ILogger<CosmosRepository> _logger;

        public CosmosRepository(IConfiguration config, ILogger<CosmosRepository> logger)
        {
            _logger = logger;

            var databaseId = config.GetValue<string>("CosmosDB:DatabaseId");

            Container = ApiStartup.CosmosClient.GetContainer(databaseId, "main");
        }

        public async Task<T?> Get<T>(DocumentType type, string? id, CancellationToken cancellationToken) where T : CosmosDocument
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            try
            {
                var response = await Container.ReadItemAsync<T>($"{type}:{id}", new PartitionKey($"{type}:{id}"), CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

                if (response.RequestCharge > 1.7)
                {
                    _logger.LogWarning("Get - ID {0}, RequestCharge {1}", id, response.RequestCharge);
                }

                return response.Resource;
            }
            catch (CosmosOperationCanceledException)
            {
                return null;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<List<T>> ListAll<T>(DocumentType Type, CancellationToken cancellationToken) where T : MainDocument
        {
            try
            {
                var query = Container
                   .GetItemLinqQueryable<T>(requestOptions: CosmosRepositoryExtensions.GetQueryRequestOptions())
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

                if (charges > 7)
                {
                    _logger.LogWarning("ListAll - Type {Type}, RequestCharge {Charges}", Type.ToString(), charges);
                }

                return results;
            }
            catch (CosmosOperationCanceledException)
            {
                return [];
            }
        }

        public async Task<List<T>> Query<T>(Expression<Func<T, bool>> predicate, DocumentType Type, CancellationToken cancellationToken) where T : MainDocument
        {
            try
            {
                var query = Container
                .GetItemLinqQueryable<T>(requestOptions: CosmosRepositoryExtensions.GetQueryRequestOptions())
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

                if (charges > 7)
                {
                    _logger.LogWarning("Query - Type {Type}, RequestCharge {Charges}", Type.ToString(), charges);
                }

                return results;
            }
            catch (CosmosOperationCanceledException)
            {
                return [];
            }
        }

        public async Task<T> Upsert<T>(T item, CancellationToken cancellationToken) where T : CosmosDocument, new()
        {
            try
            {
                var response = await Container.UpsertItemAsync(item, new PartitionKey(item.Id), CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

                if (response.RequestCharge > 15)
                {
                    _logger.LogWarning("Upsert - ID {Id}, RequestCharge {Charges}", item.Id, response.RequestCharge);
                }

                return response.Resource;
            }
            catch (CosmosOperationCanceledException)
            {
                return new T();
            }
        }

        public async Task<T> PatchItem<T>(DocumentType type, string? id, List<PatchOperation> operations, CancellationToken cancellationToken) where T : CosmosDocument, new()
        {
            //https://learn.microsoft.com/en-us/azure/cosmos-db/partial-document-update-getting-started?tabs=dotnet

            try
            {
                var response = await Container.PatchItemAsync<T>($"{type}:{id}", new PartitionKey($"{type}:{id}"), operations, CosmosRepositoryExtensions.GetPatchItemRequestOptions(), cancellationToken);

                if (response.RequestCharge > 15)
                {
                    _logger.LogWarning("PatchItem - ID {Id}, RequestCharge {Charges}", id, response.RequestCharge);
                }

                return response.Resource;
            }
            catch (CosmosOperationCanceledException)
            {
                return new T();
            }
        }

        public async Task<bool> Delete<T>(T item, CancellationToken cancellationToken) where T : CosmosDocument
        {
            try
            {
                var response = await Container.DeleteItemAsync<T>(item.Id, new PartitionKey(item.Id), CosmosRepositoryExtensions.GetItemRequestOptions(), cancellationToken);

                if (response.RequestCharge > 15)
                {
                    _logger.LogWarning("Delete - ID {Id}, RequestCharge {Charges}", item.Id, response.RequestCharge);
                }

                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (CosmosOperationCanceledException)
            {
                return false;
            }
        }

        //Overview of indexing in Azure Cosmos DB
        //https://learn.microsoft.com/en-us/azure/cosmos-db/index-overview
    }
}