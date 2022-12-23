using System.ComponentModel.DataAnnotations;

namespace SD.Shared.Models.Support
{
    public class TicketModel : DocumentBase
    {
        public TicketModel() : base(DocumentType.Ticket, true)
        {
        }

        public string? IdUserOwner { get; set; }

        [Required]
        [Custom(Name = "Título", Prompt = "Uma frase que resume seu feedback")]
        public string? Title { get; set; }

        [Required]
        [Custom(Name = "Descrição", Prompt = "Descreva o mais detalhado possível para que possamos entender melhor a situação")]
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

            DtUpdate = DateTime.UtcNow;
        }

        public override void SetIds(string? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            SetValues(Guid.NewGuid().ToString());
            IdUserOwner = id.ToString();
        }

        public void Vote(VoteType voteType)
        {
            if (voteType == VoteType.PlusOne)
                TotalVotes++;
            else
                TotalVotes--;

            DtUpdate = DateTime.UtcNow;
        }

        public override bool Equals(object? obj)
        {
            return obj is TicketModel q && q.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
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