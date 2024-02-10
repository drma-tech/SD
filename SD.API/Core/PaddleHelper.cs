using Microsoft.Azure.Functions.Worker.Http;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace SD.API.Core
{
    public static class PaddleHelper
    {
        public static async Task<bool> ValidPaddleSignature(this HttpRequestData req, string? paddleSignature, CancellationToken cancellationToken)
        {
            var paddleHeader = req.Headers.GetValues("Paddle-Signature").First();
            var ts = paddleHeader.Split(";")[0];
            var h1 = paddleHeader.Split(";")[1];
            var tsValue = ts.Split("=")[1];
            var h1Value = h1.Split("=")[1];
            var rawbody = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);
            var payload = tsValue + ":" + rawbody;

            var encoding = new UTF8Encoding();
            var keyByte = encoding.GetBytes(paddleSignature ?? throw new ArgumentNullException(nameof(paddleSignature)));
            var hmacsha256 = new HMACSHA256(keyByte);
            var messageBytes = encoding.GetBytes(payload);
            var hashmessage = hmacsha256.ComputeHash(messageBytes);
            var hash = Encoding.UTF8.GetString(hashmessage);

            return h1Value.Equals(hash, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}