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
        [Custom(Name = "TitleName", Prompt = "TitlePrompt", ResourceType = typeof(Resources.TicketModel))]
        public string? Title { get; set; }

        [Required]
        [Custom(Name = "DescriptionName", Prompt = "DescriptionPrompt", ResourceType = typeof(Resources.TicketModel))]
        public string? Description { get; set; }

        [Required]
        [Custom(Name = "Tipo")]
        public TicketType TicketType { get; set; }

        [Custom(Name = "Status")]
        public TicketStatus TicketStatus { get; set; } = TicketStatus.New;

        [Custom(Name = "Total de Votos")]
        public int TotalVotes { get; set; }

        public void ChangeStatus(TicketStatus ticketStatus)
        {
            TicketStatus = ticketStatus;

            Update();
        }

        public void Initialize(string? idUserOwner)
        {
            if (string.IsNullOrEmpty(idUserOwner)) throw new ArgumentNullException(nameof(idUserOwner));

            IdUserOwner = idUserOwner;

            var id = Guid.NewGuid().ToString();

            base.Initialize(id, id);
        }

        public void Vote(VoteType voteType)
        {
            if (voteType == VoteType.PlusOne)
                TotalVotes++;
            else
                TotalVotes--;

            Update();
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
}