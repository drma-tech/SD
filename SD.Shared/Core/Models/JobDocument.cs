using SD.Shared.Core.Types;

namespace SD.Shared.Core.Models;

public readonly record struct JobIdentity(JobType Type, string? DocId) : ICosmosIdentity
{
    public string Id => $"{Type}:{DocId?.RemovePrefix()}";
    public string? RawId => DocId?.RemovePrefix();
    public object Key => (int)Type;
}

public abstract class JobDocument(JobIdentity identity, DateTimeOffset runAt) : CosmosDocument(identity)
{
    public JobType Type { get; set; } = identity.Type;

    public DateTimeOffset RunAt { get; set; } = runAt;
}
