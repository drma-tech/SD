namespace SD.Shared.Models
{
    public class CacheModel<TData> where TData : class
    {
        protected const int one_day = 60 * 60 * 24;
        protected const int one_month = 60 * 60 * 24 * 30;

        public CacheModel()
        {
        }

        public CacheModel(string key, TData data, int? ttl = one_day)
        {
            ttl ??= one_day;

            Id = key;
            Key = key;
            Ttl = ttl.Value;
            Data = data;
        }

        public string? Id { get; set; }
        public string? Key { get; set; }
        public int Ttl { get; set; }
        public virtual TData? Data { get; set; } //TODO: cosmos doesn`t support save dynamic property (yet)
    }
}