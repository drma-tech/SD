namespace SD.Shared.Core
{
    public enum CosmosType
    {
        Principal = 1,
        Provider = 2,
        MyProvider = 3,
        WishList = 4,
        WatchedList = 5
    }

    public abstract class CosmosBase
    {
        //TODO: proteger os atributos assim que a leitura dos dados na classe conseguir atribuir valores

        protected CosmosBase(CosmosType Type)
        {
            this.Type = Type;
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
        public CosmosType Type { get; set; }

        /// <summary>
        /// Record Insertion Date
        /// </summary>
        public DateTimeOffset DtInsert { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Update date of one or more fields (after insert)
        /// </summary>
        public DateTimeOffset? DtUpdate { get; set; }

        /// <summary>
        /// Set object id (will be merged with type)
        /// </summary>
        /// <param name="id"></param>
        public void SetId(string id)
        {
            Id = Type + ":" + id;
        }

        /// <summary>
        /// PartitionKeyPath (Logical Partition)
        /// <para>If it is a 'parent' structure, use the Id value</para>
        /// <para>If it is a 'child' structure, use the Id value of the 'parent' structure</para>
        /// </summary>
        public void SetPartitionKey(string id)
        {
            Key = id;
        }

        /// <summary>
        /// Set attributes of key fields (merge with Type field)
        /// </summary>
        /// <param name="IdLoggedUser">Token's captured user id</param>
        public abstract void SetIds(string? IdLoggedUser);
    }

    public class CosmosBaseQuery
    {
    }
}