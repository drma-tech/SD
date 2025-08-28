namespace SD.Shared.Models.Blocked;

public class DataBlockedCache : CacheDocument<DataBlocked>
{
    public DataBlockedCache()
    {
    }

    public DataBlockedCache(DataBlocked data, string key, TtlCache ttl) : base(key, data, ttl)
    {
    }
}