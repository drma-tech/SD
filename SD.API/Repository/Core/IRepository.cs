using Microsoft.Azure.Cosmos;
using System.Linq.Expressions;

namespace SD.API.Repository.Core
{
    public interface IRepository
    {
        Task<T?> Get<T>(string id, CancellationToken cancellationToken) where T : CosmosDocument;

        Task<List<T>> ListAll<T>(DocumentType Type, CancellationToken cancellationToken) where T : MainDocument;

        Task<List<T>> Query<T>(Expression<Func<T, bool>> predicate, DocumentType Type, CancellationToken cancellationToken) where T : MainDocument;

        Task<T> Upsert<T>(T item, CancellationToken cancellationToken) where T : CosmosDocument;

        Task<T> PatchItem<T>(string id, List<PatchOperation> operations, CancellationToken cancellationToken) where T : CosmosDocument;

        Task<bool> Delete<T>(T item, CancellationToken cancellationToken) where T : CosmosDocument;
    }
}