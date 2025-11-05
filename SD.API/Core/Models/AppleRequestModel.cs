using Newtonsoft.Json;

namespace SD.API.Core.Models
{
    public class AppleRequestModel
    {
        [JsonProperty("receipt-data")]
        public string? ReceiptData { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }

        [JsonProperty("exclude-old-transactions")]
        public bool ExcludeOldTransactions { get; set; }
    }
}