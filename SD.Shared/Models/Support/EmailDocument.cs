namespace SD.Shared.Models.Support
{
    public class EmailDocument : CosmosDocument
    {
        public EmailDocument()
        {
        }

        public EmailDocument(string key) : base(key, key)
        {
        }

        public string? Subject { get; set; }
        public string? Html { get; set; }
        public string? Text { get; set; }
        public EmailAddress? From { get; set; }
        public List<EmailAddress> To { get; set; } = [];
        public List<EmailAddress> Cc { get; set; } = [];
        public DateTime? Date { get; set; }

        public string? SenderIp { get; set; }

        public string? FromName => From?.Name;
        public string? FromEmail => From?.Email;
        public string? ToName => To.FirstOrDefault()?.Name;
        public string? ToEmail => To.FirstOrDefault()?.Email;

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

    public class EmailAddress
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}