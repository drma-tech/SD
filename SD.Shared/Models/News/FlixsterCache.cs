namespace SD.Shared.Models.News
{
    public class FlixsterCache : CacheModel<Flixster>
    {
        public FlixsterCache()
        {
        }

        public FlixsterCache(string key, Flixster data, int? ttl = one_day)
        {
            ttl ??= one_day;

            Id = key;
            Key = key;
            Ttl = ttl.Value;
            Data = data;
        }
    }
}