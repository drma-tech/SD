using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using SD.API.Repository.Core;

namespace SD.API.Repository;

public class CosmosCacheRepository(CosmosClient client, ILogger<CosmosCacheRepository> logger)
     : BaseRepository<CosmosCacheRepository, CacheDocument, CacheIdentity>(client, logger, "cache")
{
}
