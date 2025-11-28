using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SD.API.Core
{
    public static class AppleJwtDecoder
    {
        public static AppleNotification DecodeServerNotification(string signedPayload, Apple config)
        {
            var payloadJson = ValidateAndReadJwt(signedPayload);
            return JsonSerializer.Deserialize<AppleNotification>(payloadJson) ?? throw new NotificationException("Invalid AppleNotification JSON");
        }

        public static AppleTransactionInfo DecodeTransaction(string signedTransactionInfo)
        {
            var payloadJson = ValidateAndReadJwt(signedTransactionInfo);
            return JsonSerializer.Deserialize<AppleTransactionInfo>(payloadJson) ?? throw new NotificationException("Invalid AppleTransactionInfo JSON");
        }

        private static string ValidateAndReadJwt(string jwt)
        {
            var token = new JwtSecurityToken(jwt);

            if (!token.Header.TryGetValue("x5c", out var x5cObj)) throw new NotificationException("x5c missing");
            var x5cList = (x5cObj as IEnumerable<object>)?.Select(x => x.ToString()).ToList() ?? throw new NotificationException("Invalid x5c");

            var cert = X509CertificateLoader.LoadCertificate(Convert.FromBase64String(x5cList[0]!));
            var ecdsa = cert.GetECDsaPublicKey() ?? throw new NotificationException("Not an ES256 certificate");

            var parts = jwt.Split('.');
            if (parts.Length != 3) throw new NotificationException("Invalid JWT");

            var message = Encoding.UTF8.GetBytes(parts[0] + "." + parts[1]);
            var signature = Base64UrlDecode(parts[2]);  // ← Base64Url, não normal

            var ok = ecdsa.VerifyData(message, signature, HashAlgorithmName.SHA256);
            if (!ok) throw new NotificationException("Invalid signature");

            var payload = Base64UrlDecode(parts[1]);
            return Encoding.UTF8.GetString(payload);
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string s = input.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            return Convert.FromBase64String(s);
        }
    }

    public class AppleNotification
    {
        [JsonPropertyName("notificationType")]
        public string NotificationType { get; set; } = "";

        [JsonPropertyName("subtype")]
        public string? Subtype { get; set; }

        [JsonPropertyName("data")]
        public AppleNotificationData Data { get; set; } = new();
    }

    public class AppleNotificationData
    {
        [JsonPropertyName("appAppleId")]
        public long AppAppleId { get; set; }

        [JsonPropertyName("bundleId")]
        public string BundleId { get; set; } = "";

        [JsonPropertyName("environment")]
        public string Environment { get; set; } = "";

        [JsonPropertyName("signedTransactionInfo")]
        public string SignedTransactionInfo { get; set; } = "";

        [JsonPropertyName("signedRenewalInfo")]
        public string? SignedRenewalInfo { get; set; }
    }

    public class AppleTransactionInfo
    {
        [JsonPropertyName("originalTransactionId")]
        public string OriginalTransactionId { get; set; } = "";

        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; } = "";

        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = "";

        [JsonPropertyName("expiresDate")]
        public long ExpiresDate { get; set; }

        [JsonPropertyName("purchaseDate")]
        public long PurchaseDate { get; set; } 

        [JsonPropertyName("signedDate")]
        public long SignedDate { get; set; }
    }
}