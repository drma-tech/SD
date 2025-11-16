using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
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
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            // 1) Montar cadeia de certificados do header x5c
            var x5c = token.Header["x5c"] as IEnumerable<object> ?? throw new NotificationException("x5c header missing");

            //var certificates = x5c.Select(c => new X509Certificate2(Convert.FromBase64String(c.ToString()!))).ToList();
            var certificates = x5c.Select(c => X509CertificateLoader.LoadCertificate(Convert.FromBase64String(c.ToString()!))).ToList();

            // Primeiro certificado assina o JWT
            var signingCert = certificates[0];

            var validation = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // Apple frequentemente manda timestamps quase simultâneos
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new X509SecurityKey(signingCert)
            };

            handler.ValidateToken(jwt, validation, out var validated);

            var validatedToken = (JwtSecurityToken)validated;

            // Payload é um JSON normal
            return validatedToken.Payload.SerializeToJson();
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

        // está SEMPRE em milissegundos
        [JsonPropertyName("expiresDate")]
        public string ExpiresDate { get; set; } = "";

        [JsonPropertyName("purchaseDate")]
        public string PurchaseDate { get; set; } = "";

        [JsonPropertyName("signedDate")]
        public long SignedDate { get; set; }
    }
}