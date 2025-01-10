using System.ComponentModel.DataAnnotations;

namespace SD.Shared.Models.Support
{
    public class TicketModel : ProtectedMainDocument
    {
        public TicketModel() : base(DocumentType.Ticket)
        {
        }

        public string? IdUserOwner { get; set; }

        [Required]
        [Custom(Name = "TitleName", Placeholder = "TitlePrompt", ResourceType = typeof(Resources.TicketModel))]
        public string? Title { get; set; }

        [Required]
        [Custom(Name = "DescriptionName", Placeholder = "DescriptionPrompt", ResourceType = typeof(Resources.TicketModel))]
        public string? Description { get; set; }

        [Required]
        [Custom(Name = "Tipo")]
        public TicketType TicketType { get; set; }

        [Custom(Name = "Status")]
        public TicketStatus TicketStatus { get; set; } = TicketStatus.New;

        [Custom(Name = "Total de Votos")]
        public List<TicketVote> Votes { get; set; } = [];

        public int TotalVotes => Votes.Sum(s => s.VoteType == VoteType.MinusOne ? -1 : 1);

        public void ChangeStatus(TicketStatus ticketStatus)
        {
            TicketStatus = ticketStatus;
        }

        public new void Initialize(string? idUserOwner)
        {
            if (string.IsNullOrEmpty(idUserOwner)) throw new ArgumentNullException(nameof(idUserOwner));

            IdUserOwner = idUserOwner;

            var id = Guid.NewGuid().ToString();

            base.Initialize(id);
        }

        public void Vote(string userId, VoteType voteType)
        {
            Votes.Add(new TicketVote { IdVotedUser = userId, VoteType = voteType });
        }

        public override bool Equals(object? obj)
        {
            return obj is TicketModel q && q.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public override bool HasValidData()
        {
            return !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Description);
        }
    }

    public class TicketVote
    {
        public string? IdVotedUser { get; set; }

        public VoteType VoteType { get; set; }
    }

    public enum TicketType
    {
        [Custom(Name = "BugReport", ResourceType = typeof(Resources.TicketType))]
        BugReport = 1,

        [Custom(Name = "Improvement", ResourceType = typeof(Resources.TicketType))]
        Improvement = 2
    }

    public enum TicketStatus
    {
        [Custom(Name = "New", ResourceType = typeof(Resources.TicketStatus))]
        New = 1,

        [Custom(Name = "UnderConsideration", ResourceType = typeof(Resources.TicketStatus))]
        UnderConsideration = 2,

        [Custom(Name = "Planned", ResourceType = typeof(Resources.TicketStatus))]
        Planned = 3,

        [Custom(Name = "Progress", ResourceType = typeof(Resources.TicketStatus))]
        Progress = 4,

        [Custom(Name = "Done", ResourceType = typeof(Resources.TicketStatus))]
        Done = 5,

        [Custom(Name = "Declined", ResourceType = typeof(Resources.TicketStatus))]
        Declined = 6
    }

    public enum VoteType
    {
        MinusOne = -1,
        PlusOne = 1
    }
}