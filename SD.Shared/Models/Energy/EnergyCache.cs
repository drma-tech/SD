namespace SD.Shared.Models.Energy;

public class EnergyCache : CacheDocument<EnergyModel>
{
    public EnergyCache()
    {
    }

    public EnergyCache(EnergyModel data, string key) : base(key, data, TtlCache.OneDay)
    {
    }
}