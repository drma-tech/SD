namespace SD.Shared.Core.Models
{
    public enum DocumentType
    {
        Principal = 1,
        //Provider = 2,
        MyProvider = 3,
        WishList = 4,
        WatchedList = 5,
        Ticket = 6,
        //TicketVote = 7,
        //Announcement = 8,
        WatchingList = 9,
        Login = 10,
        MySuggestions = 11,
        Update = 12
    }

    public abstract class MainDocument : CosmosDocument
    {
        protected MainDocument(DocumentType type)
        {
            Type = type;
        }

        protected MainDocument(string id, DocumentType type) : base($"{type}:{id}")
        {
            Type = type;
        }

        public DocumentType Type { get; set; }
    }

    /// <summary>
    /// Public read and private write
    /// </summary>
    public abstract class ProtectedMainDocument : MainDocument
    {
        private readonly DocumentType type;

        protected ProtectedMainDocument(DocumentType type) : base(type)
        {
            this.type = type;
        }

        protected ProtectedMainDocument(string id, DocumentType type) : base($"{type}:{id}", type)
        {
            this.type = type;
        }

        public virtual void Initialize(string id)
        {
            SetIds($"{type}:{id}");
        }
    }

    /// <summary>
    /// Private read and write
    /// </summary>
    public abstract class PrivateMainDocument : MainDocument
    {
        private readonly DocumentType type;

        protected PrivateMainDocument(DocumentType type) : base(type)
        {
            this.type = type;
        }

        public virtual void Initialize(string userId)
        {
            SetIds($"{type}:{userId}");
        }
    }
}