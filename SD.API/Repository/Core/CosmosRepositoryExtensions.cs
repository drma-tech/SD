using Microsoft.Azure.Cosmos;

namespace SD.API.Repository.Core
{
    public static class CosmosRepositoryExtensions
    {
        public static ItemRequestOptions? GetItemRequestOptions()
        {
            return null;
        }

        public static PatchItemRequestOptions? GetPatchItemRequestOptions()
        {
            return null;
        }

        public static QueryRequestOptions? GetQueryRequestOptions(PartitionKey? key, bool enableMetrics)
        {
            return new QueryRequestOptions()
            {
                PartitionKey = key,

                //https://learn.microsoft.com/en-us/training/modules/measure-index-azure-cosmos-db-sql-api/4-measure-query-cost
                MaxItemCount = 10, // - max itens per page

                //https://learn.microsoft.com/en-us/training/modules/measure-index-azure-cosmos-db-sql-api/2-enable-indexing-metrics
                PopulateIndexMetrics = enableMetrics //enable only when analysing metrics
            };
        }
    }
}