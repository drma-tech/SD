using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using SD.API.Repository.Core;

namespace SD.API.Repository;

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
    public DateTimeOffset DateTime { get; set; } = DateTimeOffset.Now;

    [JsonInclude] public int Ttl { get; init; }
}

public class CosmosLogRepository
{
    public CosmosLogRepository(IConfiguration config)
    {
        var databaseId = config.GetValue<string>("CosmosDB:DatabaseId");

        Container = ApiStartup.CosmosClient.GetContainer(databaseId, "logs");
    }

    public Container Container { get; }

    public async Task Add(LogModel log)
    {
        await Container.CreateItemAsync(log, new PartitionKey(log.Id),
            CosmosRepositoryExtensions.GetItemRequestOptions());
    }
}