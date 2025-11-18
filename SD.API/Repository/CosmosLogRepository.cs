using Microsoft.Azure.Cosmos;
using SD.API.Repository.Core;
using SD.API.StartupLogging;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SD.API.Repository;

public class LogModel
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? State { get; set; }
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public DateTimeOffset DateTime { get; set; } = DateTimeOffset.UtcNow;

    [JsonInclude] public int Ttl { get; init; }
}

public class CosmosLogRepository
{
    public Container Container { get; }

    public CosmosLogRepository(CosmosClient CosmosClient)
    {
        var swCosmos = Stopwatch.StartNew();

        var databaseId = ApiStartup.Configurations.CosmosDB?.DatabaseId;

        Container = CosmosClient.GetContainer(databaseId, "logs");

        swCosmos.Stop();
        StartupLogBuffer.Enqueue($"CosmosLogRepository {swCosmos.Elapsed}");
    }

    public async Task Add(LogModel log)
    {
        await Container.CreateItemAsync(log, new PartitionKey(log.Id), CosmosRepositoryExtensions.GetItemRequestOptions());
    }
}
