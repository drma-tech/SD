using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace SD.Shared.Models.Auth
{
    public class AuthSubscription
    {
        public string? SubscriptionId { get; set; }
        public string? CustomerId { get; set; }
        public bool Active { get; set; } = false;

        public AccountProduct? Product { get; set; }
        public AccountCycle? Cycle { get; set; }

        [JsonIgnore]
        [NotMapped]
        public AccountProduct ActiveProduct => Active ? Product ?? AccountProduct.Basic : AccountProduct.Basic;
    }
}