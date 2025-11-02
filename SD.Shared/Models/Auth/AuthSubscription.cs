using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD.Shared.Models.Auth
{
    public class AuthSubscription
    {
        public string? SubscriptionId { get; set; }
        public string? CustomerId { get; set; }
        public bool IsPaidUser { get; set; } = false;

        public AccountProduct? Product { get; set; }

        [JsonIgnore]
        [NotMapped]
        public AccountProduct ActiveProduct => IsPaidUser ? Product ?? AccountProduct.Basic : AccountProduct.Basic;
    }
}