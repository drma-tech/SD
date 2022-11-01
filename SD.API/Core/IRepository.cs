using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Core
{
    public interface IRepository
    {
        Task<T?> Get<T>(string? id, string? partitionKeyValue, CancellationToken cancellationToken) where T : DocumentBase;

        Task<List<T>> Query<T>(Expression<Func<T, bool>> predicate, string partitionKeyValue, DocumentType Type, CancellationToken cancellationToken) where T : DocumentBase;

        Task<List<T>> Query<T>(QueryDefinition query, CancellationToken cancellationToken) where T : DocumentBaseQuery;

        Task<T> Upsert<T>(T item, CancellationToken cancellationToken) where T : DocumentBase;

        Task<T> PatchItem<T>(string id, string partitionKeyValue, List<PatchOperation> operations, CancellationToken cancellationToken) where T : DocumentBase;

        Task<bool> Delete<T>(T item, CancellationToken cancellationToken) where T : DocumentBase;
    }
}