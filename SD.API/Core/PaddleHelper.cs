using Microsoft.Azure.Functions.Worker.Http;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Reflection;
using System.Text;

namespace SD.API.Core
{
    public static class PaddleHelper
    {
        public static bool PaddleWebhookVerify(this HttpHeadersCollection header)
        {
            SortedDictionary<string, dynamic> padStuff = [];
            PhpSerializer serializer = new();
            byte[] signature = Convert.FromBase64String(header.GetValues("p_signature")?.FirstOrDefault() ?? "");

            var assy = Assembly.GetAssembly(typeof(PaddleHelper));
            var stream = assy?.GetManifestResourceStream(typeof(PaddleHelper), "paddle_pub_key.pem") ?? throw new NotificationException("paddle_pub_key.pem");
            var reader = new StreamReader(stream);
            string publicKey = reader.ReadToEnd();

            foreach (string key in header.SelectMany(s => s.Value))
            {
                var myVal = header.GetValues(key)?.FirstOrDefault() ?? "";
                if (key != "p_signature")
                {
                    padStuff.Add(key, myVal);
                }
            }

            string payload = serializer.Serialize(padStuff);

            return verifySignature(signature, payload, publicKey);
        }

        private static bool verifySignature(byte[] signatureBytes, string message, string pubKey)
        {
            StringReader newStringReader = new(pubKey);
            AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)new PemReader(newStringReader).ReadObject();
            ISigner sig = SignerUtilities.GetSigner("SHA1withRSA");
            sig.Init(false, publicKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            sig.BlockUpdate(messageBytes, 0, messageBytes.Length);
            return sig.VerifySignature(signatureBytes);
        }
    }
}