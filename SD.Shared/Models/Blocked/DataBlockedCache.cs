namespace SD.Shared.Models.Blocked;

public class DataBlockedCache(string id, DataBlocked data) : CacheDocumentData<DataBlocked>(new CacheIdentity(id), data, TtlCache.OneWeek)
{
}

public class DataBlocked
{
    public int Quantity { get; set; } = 1;
}