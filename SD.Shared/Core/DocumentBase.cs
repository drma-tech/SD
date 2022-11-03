namespace SD.Shared.Core
{
    public enum DocumentType
    {
        Principal = 1,
        Provider = 2,
        MyProvider = 3,
        WishList = 4,
        WatchedList = 5,
        Ticket = 6,
        TicketVote = 7,
    }

    public abstract class DocumentBase
    {
        //TODO: proteger os atributos assim que a leitura dos dados na classe conseguir atribuir valores

        protected DocumentBase(DocumentType Type, bool Samekey)
        {
            this.Type = Type;
            this.Samekey = Samekey;
        }

        /// <summary>
        /// Single field within the container (has distinction by type)
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// PartitionKeyPath (Logical Partition)
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Structure type (usually the class name)
        /// </summary>
        public DocumentType Type { get; set; }

        /// <summary>
        /// Record Insertion Date
        /// </summary>
        public DateTimeOffset DtInsert { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Update date of one or more fields (after insert)
        /// </summary>
        public DateTimeOffset? DtUpdate { get; set; } = null;

        /// <summary>
        /// Key have the same identification as the Id (Id=type:12345, Key=12345)
        /// </summary>
        private bool Samekey { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id">Unique Identification</param>
        /// <param name="key">Logical Partition</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        protected void SetValues(string id, string? key = null)
        {
            if (!Samekey && key == null) throw new ArgumentNullException(nameof(key));
            if (Samekey && key != null && id != key) throw new ArgumentException("parameters must be the same");
            if (!Samekey && id == key) throw new ArgumentException("parameters must be the different");

            Id = $"{Type}:{id}";
            Key = Samekey ? id : key;
        }

        /// <summary>
        /// Set attributes of key fields (merge with Type field)
        /// </summary>
        /// <param name="id">Unique Identification</param>
        public abstract void SetIds(string id);
    }

    public class DocumentBaseQuery
    {
    }
}