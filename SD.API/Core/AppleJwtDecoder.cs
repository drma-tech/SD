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
        private const string AppleRootCaG3Sha256 = "63343ABFB89A6A03EBB57E9B3F5FA7BE7C4F5C756F3017B3A8C488C3653E9179";

        public static AppleNotification DecodeServerNotification(string signedPayload, Apple config, string expectedEnvironment)
        {
            var payloadJson = ValidateAndReadJwt(signedPayload);
            var notification = JsonSerializer.Deserialize<AppleNotification>(payloadJson) ?? throw new UnhandledException("Invalid AppleNotification JSON");

            ValidateApp(notification.Data.BundleId, notification.Data.Environment, config.BundleId, expectedEnvironment);

            return notification;
        }

        public static AppleTransactionInfo DecodeTransaction(string signedTransactionInfo, string bundleId, string environment)
        {
            var payloadJson = ValidateAndReadJwt(signedTransactionInfo);
            var transaction = JsonSerializer.Deserialize<AppleTransactionInfo>(payloadJson) ?? throw new UnhandledException("Invalid AppleTransactionInfo JSON");

            ValidateApp(transaction.BundleId, transaction.Environment, bundleId, environment);

            return transaction;
        }

        private static void ValidateApp(string actualBundleId, string actualEnvironment, string? expectedBundleId, string expectedEnvironment)
        {
            if (!string.Equals(actualBundleId, expectedBundleId, StringComparison.OrdinalIgnoreCase))
                throw new UnhandledException("Invalid Apple bundle ID");

            if (!string.Equals(actualEnvironment, expectedEnvironment, StringComparison.OrdinalIgnoreCase))
                throw new UnhandledException("Invalid Apple environment");
        }

        private static string ValidateAndReadJwt(string jwt)
        {
            var token = new JwtSecurityToken(jwt);

            if (!token.Header.TryGetValue("x5c", out var x5cObj)) throw new UnhandledException("x5c missing");
            if (!string.Equals(token.Header.Alg, "ES256", StringComparison.Ordinal))
                throw new UnhandledException("Invalid JWT algorithm");

            var x5cList = (x5cObj as IEnumerable<object>)?.Select(x => x.ToString()).ToList() ?? throw new UnhandledException("Invalid x5c");
            if (x5cList.Count != 3) throw new UnhandledException("Invalid Apple certificate chain");

            var certificates = x5cList.Select(x => X509CertificateLoader.LoadCertificate(Convert.FromBase64String(x!))).ToList();

            try
            {
                var leaf = certificates[0];
                var root = certificates[^1];

                if (!string.Equals(root.GetCertHashString(HashAlgorithmName.SHA256), AppleRootCaG3Sha256, StringComparison.OrdinalIgnoreCase))
                    throw new UnhandledException("Certificate chain is not rooted in Apple Root CA G3");

                using var chain = new X509Chain();
                chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                chain.ChainPolicy.CustomTrustStore.Add(root);
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.ExtraStore.Add(certificates[1]);

                if (!chain.Build(leaf))
                    throw new UnhandledException("Invalid Apple certificate chain");

                using var ecdsa = leaf.GetECDsaPublicKey() ?? throw new UnhandledException("Not an ES256 certificate");

                var parts = jwt.Split('.');
                if (parts.Length != 3) throw new UnhandledException("Invalid JWT");

                var message = Encoding.UTF8.GetBytes(parts[0] + "." + parts[1]);
                var signature = Base64UrlDecode(parts[2]);

                if (!ecdsa.VerifyData(message, signature, HashAlgorithmName.SHA256))
                    throw new UnhandledException("Invalid signature");

                var payload = Base64UrlDecode(parts[1]);
                return Encoding.UTF8.GetString(payload);
            }
            finally
            {
                foreach (var certificate in certificates)
                {
                    certificate.Dispose();
                }
            }
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
        [JsonPropertyName("bundleId")]
        public string BundleId { get; set; } = "";

        [JsonPropertyName("environment")]
        public string Environment { get; set; } = "";

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