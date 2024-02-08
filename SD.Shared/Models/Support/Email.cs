using StrongGrid.Models.Webhooks;

namespace SD.Shared.Models.Support
{
    public class Email : CosmosDocument
    {
        public Email()
        {
        }

        public Email(string key, InboundEmail? email) : base(key, key)
        {
            InboundEmail = email;
        }

        public InboundEmail? InboundEmail { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is AnnouncementModel q && q.Id == Id;
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