using System.Text.Json.Serialization;

namespace SD.Shared.Core.Models;

public class CacheDocument<TData> : CosmosDocument where TData : class, new()
{
    public CacheDocument()
    {
    }

    public CacheDocument(string id, TData? data, TtlCache ttl) : base(id)
    {
        Data = data;
        Ttl = (int)ttl;
    }

    [JsonInclude] public TData? Data { get; init; } //TODO: cosmos doesn't support save dynamic property (yet)

    [JsonInclude] public int Ttl { get; init; }
}
