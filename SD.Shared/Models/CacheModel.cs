namespace SD.Shared.Models
{
    public class CacheModel<TData> where TData : class
    {
        public CacheModel()
        {
        }

        public CacheModel(string key, TData data, ttlCache ttl)
        {
            Id = key;
            Key = key;
            Ttl = (int)ttl;
            Data = data;
        }

        public string? Id { get; set; }
        public string? Key { get; set; }
        public int Ttl { get; set; }
        public virtual TData? Data { get; set; } //TODO: cosmos doesn`t support save dynamic property (yet)
    }
}