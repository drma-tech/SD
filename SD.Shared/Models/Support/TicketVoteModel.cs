using SD.Shared.Core.Models;

namespace SD.Shared.Models.Support
{
    public class TicketVoteModel : ProtectedMainDocument
    {
        public TicketVoteModel() : base(DocumentType.TicketVote)
        {
        }

        public string? IdVotedUser { get; set; }

        public VoteType VoteType { get; set; }

        public override void Initialize(string id, string key)
        {
            base.Initialize(Guid.NewGuid().ToString(), key);
        }

        public override bool HasValidData()
        {
            return !string.IsNullOrEmpty(IdVotedUser);
        }

        public override bool Equals(object? obj)
        {
            return obj is TicketVoteModel q && q.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }
    }

    public enum VoteType
    {
        MinusOne = -1,
        PlusOne = 1
    }
}