using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SD.API.Core
{
    public static class ZeptoMailVerify
    {
        public static bool VerifyZeptoWebhook(string rawBody, string producerSignature, string secret)
        {
            if (string.IsNullOrWhiteSpace(producerSignature))
                return false;

            var decoded = HttpUtility.UrlDecode(producerSignature);

            var parts = decoded.Split(';')
                .Select(x => x.Split('=', 2))
                .ToDictionary(x => x[0], x => x[1]);

            if (!parts.TryGetValue("ts", out var ts))
                return false;

            if (!parts.TryGetValue("s", out var receivedSignature))
                return false;

            if (!parts.TryGetValue("s-algorithm", out var algorithm))
                return false;

            // replay protection
            var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(ts));
            var age = DateTimeOffset.UtcNow - timestamp;

            if (age > TimeSpan.FromMinutes(5))
                return false;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));

            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawBody));

            var computedSignature = Convert.ToBase64String(hash);

            return CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(receivedSignature), Convert.FromBase64String(computedSignature));
        }
    }
}