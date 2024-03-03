using StrongGrid.Models.Webhooks;

namespace SD.Shared.Models.Support
{
    public class EmailDocument : CosmosDocument
    {
        public EmailDocument()
        {
        }

        public EmailDocument(string key, InboundEmail? email) : base(key, key)
        {
            InboundEmail = email;
        }

        public InboundEmail? InboundEmail { get; set; }

        public string? From => InboundEmail?.From.Email;
        public string? To => InboundEmail?.To.FirstOrDefault()?.Email;
        public string? Subject => InboundEmail?.Subject;
        public DateTime? Date => DateTime.Parse(InboundEmail?.Headers.SingleOrDefault(w => w.Key == "Date").Value);
        public string? Html => InboundEmail?.Html;

        public override bool Equals(object? obj)
        {
            return obj is EmailDocument q && q.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public override bool HasValidData()
        {
            return true;
        }
    }
}