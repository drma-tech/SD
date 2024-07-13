using System.Text.Json.Serialization;

namespace SD.Shared.Core.Models
{
    public class CacheDocument<TData> : CosmosDocument where TData : class
    {
        protected CacheDocument()
        {
        }

        protected CacheDocument(string id, TData data, ttlCache ttl) : base(id)
        {
            Data = data;
            Ttl = (int)ttl;
        }

        [JsonInclude]
        public virtual TData? Data { get; init; } //TODO: cosmos doesn`t support save dynamic property (yet)

        [JsonInclude]
        public int Ttl { get; init; }

        public override bool HasValidData()
        {
            return Data != null;
        }
    }
}