namespace SD.Shared.Models.Auth
{
    public class TempClientePaddle
    {
        public string? CustomerId { get; set; }
        public string? AddressId { get; set; }
        public string? ProductId { get; set; }
        public string? PriceId { get; set; }
        public string? TransactionId { get; set; }
    }

    public class ClientePaddle
    {
        public string? CustomerId { get; set; }
        public string? AddressId { get; set; }

        public List<PaddleItem> Items { get; set; } = [];
        public List<PaddleTransaction> Transactions { get; set; } = [];
    }

    public class PaddleItem
    {
        public string? ProductId { get; set; }
        public string? PriceId { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;

        public AccountProduct Product { get; set; }
        public bool Active { get; set; } = false;
    }

    public class PaddleTransaction
    {
        public string? TransactionId { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;
    }
}