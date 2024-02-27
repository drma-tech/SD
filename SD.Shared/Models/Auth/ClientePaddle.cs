namespace SD.Shared.Models.Auth
{
    public class TempClientePaddle
    {
        public string? CustomerId { get; set; }
        public string? AddressId { get; set; }
        public string? ProductId { get; set; }
    }

    public class ClientePaddle
    {
        public string? SubscriptionId { get; set; }
        public string? CustomerId { get; set; }
        public string? AddressId { get; set; }
        public bool IsPaidUser { get; set; } = false;

        public List<PaddleItem> Items { get; set; } = [];
        public AccountProduct ActiveProduct => IsPaidUser ? Items.FirstOrDefault()?.Product ?? AccountProduct.Basic : AccountProduct.Basic;
    }

    public class PaddleItem
    {
        public string? ProductId { get; set; }
        public AccountProduct Product { get; set; }
    }
}