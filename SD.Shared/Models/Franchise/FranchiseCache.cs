namespace SD.Shared.Models.Franchise;

public class FranchiseCache(string id, FranchiseData data) : CacheDocumentData<FranchiseData>(new CacheIdentity(id), data, TtlCache.NeverExpire);

public class FranchiseData
{
    public ISet<FranchiseItem> FranchiseItems { get; set; } = new HashSet<FranchiseItem>();
}

public class FranchiseItem : EqualityBase<FranchiseItem>
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Poster { get; init; }
    public DateTime? LastReleaseDate { get; set; }

    protected override object?[] EqualityValues => [Id];
}