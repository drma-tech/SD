using Microsoft.Azure.Cosmos;

namespace SD.API.Repository.Core;

public static class CosmosRepositoryExtensions
{
    public static QueryRequestOptions GetQueryRequestOptions(PartitionKey? key = null)
    {
        return new QueryRequestOptions
        {
            //https://learn.microsoft.com/en-us/training/modules/measure-index-azure-cosmos-db-sql-api/4-measure-query-cost
            MaxItemCount = 250, // - max items per page
            PartitionKey = key
        };
    }
}