using System.Text;
using System.Text.Json;

namespace SD.API.Core
{
    public class ZeptoMailClient(IHttpClientFactory factory, string apiKey)
    {
        private const string _uri = "https://api.zeptomail.com/v1.1/email/template";
        private const string domain = "streamingdiscovery";
        private const string appName = "Streaming Discovery";
        private const string appSubtitle = "Discover Movies and Series on Streaming Platforms";
        private static string noreplyEmail => $"noreply@{domain}.com";
        private static string year => DateTime.Now.Year.ToString(System.Globalization.CultureInfo.InvariantCulture);

        public async Task SendWelcomeTemplate(string user_id, string address, string? name, CancellationToken cancellationToken)
        {
            name ??= address.Split("@")[0];

            var payload = new
            {
                template_alias = "welcome",
                from = new { address = noreplyEmail, name = appName },
                to = new[] { new { email_address = new { address, name } } },
                merge_info = new
                {
                    product_title = appName,
                    product_subtitle = appSubtitle,
                    domain,
                    year,
                    user_name = name,
                },
                client_reference = user_id,
            };

            var json = JsonSerializer.Serialize(payload);

            var request = new HttpRequestMessage(HttpMethod.Post, _uri);

            request.Headers.Add("Authorization", apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = factory.CreateClient();
            var response = await client.SendAsync(request, cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new NotificationException($"ZeptoMail error: {response.StatusCode} - {body}");
            }
        }
    }
}