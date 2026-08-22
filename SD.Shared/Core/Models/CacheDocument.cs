namespace SD.Shared.Core.Models;

using Json = System.Text.Json.Serialization;

public readonly record struct CacheIdentity(string? DocId) : ICosmosIdentity
{
    public string Id => DocId!;
    public string? RawId => DocId?.RemovePrefix();
    public object Key => Id;
}

public class CacheDocument(CacheIdentity identity, TtlCache ttl) : CosmosDocument(identity)
{
    [Json.JsonInclude]
    public TtlCache Ttl { get; init; } = ttl;

    protected override object?[] EqualityValues => [Id];
}

public class CacheDocumentData<T>(CacheIdentity identity, T? data, TtlCache ttl) : CacheDocument(identity, ttl) where T : class
{
    [Json.JsonInclude]
    public T? Data { get; init; } = data;
}