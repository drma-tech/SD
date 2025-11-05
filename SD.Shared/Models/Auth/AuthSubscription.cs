using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD.Shared.Models.Auth
{
    public class AuthSubscription
    {
        public string? SubscriptionId { get; set; }
        public string? CustomerId { get; set; }
        public string? LatestReceipt { get; set; }
        public DateTimeOffset? ExpiresDate { get; set; }
        public bool Active { get; set; } = false;

        public PaymentProvider? Provider { get; set; }
        public AccountProduct? Product { get; set; }
        public AccountCycle? Cycle { get; set; }

        [JsonIgnore]
        [NotMapped]
        public AccountProduct ActiveProduct => IsActive() ? Product ?? AccountProduct.Basic : AccountProduct.Basic;

        public bool IsActive()
        {
            return Provider switch
            {
                PaymentProvider.Paddle => Active,
                PaymentProvider.Microsoft => false,
                PaymentProvider.Google => false,
                PaymentProvider.Apple => ExpiresDate.HasValue && ExpiresDate.Value > DateTimeOffset.UtcNow,
                _ => false,
            };
        }
    }
}