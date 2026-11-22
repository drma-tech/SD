using SD.Common.Core.Types;

namespace SD.Common.Core.Models;

public readonly record struct MainIdentity(MainType Type, string? DocId) : ICosmosIdentity
{
    public string Id => $"{Type}:{DocId?.RemovePrefix()}";
    public string? RawId => DocId?.RemovePrefix();
    public object Key => Id;
}

public abstract class MainDocument(MainIdentity identity) : CosmosDocument(identity)
{
    public MainType Type { get; set; } = identity.Type;
}