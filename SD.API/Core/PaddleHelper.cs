using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Core;

public static class PaddleHelper
{
    public static async Task<bool> ValidPaddleSignature(this HttpRequestData req, string? paddleSignature,
        CancellationToken cancellationToken)
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
        var hash = ByteToString(hashmessage);

        return h1Value.Equals(hash, StringComparison.CurrentCultureIgnoreCase);
    }

    public static string ByteToString(byte[] buff)
    {
        var sbinary = new StringBuilder();
        for (var i = 0; i < buff.Length; i++) sbinary.Append(buff[i].ToString("X2")); // hex format
        return sbinary.ToString();
    }
}