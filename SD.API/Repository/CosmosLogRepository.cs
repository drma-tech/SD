using Microsoft.Azure.Cosmos;
using SD.API.Repository.Core;
using System.Net;
using System.Text.Json.Serialization;

namespace SD.API.Repository;

public class LogModel
{
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

public class LogDbModel
{
    public string? Id { get; set; }
    public string? OperationSystem { get; set; }
    public string? BrowserName { get; set; }
    public string? BrowserVersion { get; set; }
    public string? Platform { get; set; }
    public string? AppVersion { get; set; }
    public string? UserId { get; set; }
    public string? UserAgent { get; set; }
    public List<LogDbEvent> Events { get; set; } = [];
    [JsonInclude] public int Ttl { get; init; } = (int)TtlCache.ThreeMonths;
}

public class LogDbEvent
{
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public string? Origin { get; set; } //route or function name
    public string? Params { get; set; } //query parameters or other context info
    public string? Body { get; set; }
    public DateTimeOffset DateTime { get; set; } = DateTimeOffset.UtcNow;
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
        ItemResponse<LogDbModel>? response = null;
        var id = $"{log.Ip ?? "null-ip"}_{log.UserAgent.ToHash() ?? "null-ua"}";

        try
        {
            response = await Container.ReadItemAsync<LogDbModel>(id, new PartitionKey(id), CosmosRepositoryExtensions.GetItemRequestOptions());
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            response = null;
        }
        finally
        {
            var dbModel = response?.Resource;

            dbModel ??= new LogDbModel
            {
                Id = id,
                OperationSystem = log.OperationSystem,
                BrowserName = log.BrowserName,
                BrowserVersion = log.BrowserVersion,
                Platform = log.Platform,
                AppVersion = log.AppVersion,
                UserId = log.UserId,
                UserAgent = log.UserAgent,
            };

            dbModel.Events.Add(new LogDbEvent
            {
                Message = log.Message,
                StackTrace = log.StackTrace,
                Origin = log.Origin,
                Params = log.Params,
                Body = log.Body,
                DateTime = log.DateTime,
            });

            await Container.UpsertItemAsync(dbModel, new PartitionKey(id), CosmosRepositoryExtensions.GetItemRequestOptions());
        }
    }
}
