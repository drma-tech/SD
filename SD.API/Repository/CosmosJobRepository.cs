using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using SD.API.Repository.Core;
using SD.Shared.Core.Types;
using System.Linq.Expressions;

namespace SD.API.Repository;

public class CosmosJobRepository(CosmosClient client, ILogger<CosmosJobRepository> logger)
     : BaseRepository<CosmosJobRepository, JobDocument, JobIdentity>(client, logger, "job")
{
    public async Task<IReadOnlyCollection<T>> Query<T>(JobType type, Expression<Func<T, bool>>? predicate, Func<IQueryable<T>, IQueryable<T>>? transform, CancellationToken cancellationToken)
        where T : JobDocument
    {
        try
        {
            var queryable = Container
                .GetItemLinqQueryable<T>(requestOptions: CosmosRepositoryExtensions.GetQueryRequestOptions())
                .Where(predicate?.Compose(item => item.Type == type, Expression.AndAlso) ?? (item => item.Type == type));

            if (transform != null) queryable = transform(queryable);

            using var iterator = queryable.ToFeedIterator();
            var results = new List<T>();

            double charges = 0;
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                charges += response.RequestCharge;
                results.AddRange(response.Resource);
            }

            if (charges > 10d + extra)
                LogMessages.RequestCharge(Logger, "Query", type.ToString(), charges);

            return results;
        }
        catch (CosmosOperationCanceledException)
        {
            return [];
        }
    }
}