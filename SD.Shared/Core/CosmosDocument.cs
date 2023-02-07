using System.Text.Json.Serialization;

namespace SD.Shared.Core
{
    public abstract class CosmosDocument
    {
        private readonly bool FixedId = false;

        protected CosmosDocument()
        {
            FixedId = false;
        }

        protected CosmosDocument(string id, string key)
        {
            Id = id;
            Key = key;

            FixedId = true;
        }

        [JsonInclude]
        public string Id { get; set; } = string.Empty;

        [JsonInclude]
        public string Key { get; set; } = string.Empty;

        [JsonInclude]
        public DateTimeOffset DtInsert { get; } = DateTimeOffset.UtcNow;

        [JsonInclude]
        public DateTimeOffset? DtUpdate { get; set; } = null;

        public abstract bool HasValidData();

        public void SetIds(string id, string key)
        {
            if (FixedId) throw new InvalidOperationException();

            Id = id;
            Key = key;
        }

        public virtual void Update()
        {
            DtUpdate = DateTimeOffset.UtcNow;
        }
    }
}