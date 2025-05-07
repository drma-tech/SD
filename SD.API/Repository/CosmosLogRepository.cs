﻿using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using SD.API.Repository.Core;
using System.Text.Json.Serialization;

namespace SD.API.Repository
{
    public class LogModel
    {
        public LogModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? State { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public DateTimeOffset DateTimeEvent { get; set; } = DateTimeOffset.Now;

        [JsonInclude]
        public int Ttl { get; init; }
    }

    public class CosmosLogRepository
    {
        public Container Container { get; private set; }

        public CosmosLogRepository(IConfiguration config)
        {
            var databaseId = config.GetValue<string>("CosmosDB:DatabaseId");

            Container = ApiStartup.CosmosClient.GetContainer(databaseId, "logs");
        }

        public async Task Add(LogModel log)
        {
            await Container.CreateItemAsync(log, new PartitionKey(log.Id), CosmosRepositoryExtensions.GetItemRequestOptions());
        }
    }
}