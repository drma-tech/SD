using Microsoft.Azure.Cosmos;
using SD.API.Repository.Core;
using System.Text.Json.Serialization;

namespace SD.API.Repository;

public class LogModel
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public string? Origin { get; set; } //route or function name
    public string? Params { get; set; } //query parameters or other context info
    public string? Body { get; set; }
    public string? OperationSystem { get; set; }
    public string? BrowserName { get; set; }
    public string? BrowserVersion { get; set; }
    public string? Platform { get; set; }
    public string? AppVersion { get; set; }
    public string? UserId { get; set; }
    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
    public DateTimeOffset DateTime { get; set; } = DateTimeOffset.UtcNow;
    [JsonInclude] public int Ttl { get; init; } = (int)TtlCache.ThreeMonths;
}

public class CosmosLogRepository
{
    public Container Container { get; }

    public CosmosLogRepository(CosmosClient CosmosClient)
    {
        var databaseId = ApiStartup.Configurations.CosmosDB?.DatabaseId;

        Container = CosmosClient.GetContainer(databaseId, "logs");
    }

    public async Task Add(LogModel log)
    {
        await Container.CreateItemAsync(log, new PartitionKey(log.Id), CosmosRepositoryExtensions.GetItemRequestOptions());
    }
}