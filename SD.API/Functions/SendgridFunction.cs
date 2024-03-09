using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using SD.Shared.Models.Support;
using StrongGrid;
using StrongGrid.Models;
using StrongGrid.Models.Webhooks;
using System.Globalization;

namespace SD.API.Functions
{
    public class SendgridFunction(CosmosEmailRepository repo, IConfiguration configuration)
    {
        [Function("GetEmails")]
        public async Task<List<EmailDocument>?> GetEmails(
          [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "adm/emails")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                return await repo.Query(null, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("SendgridInbound")]
        public async Task SendgridInbound(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "public/sendgrid/inbound")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var parser = new WebhookParser();
                var inboundMail = await parser.ParseInboundEmailWebhookAsync(req.Body, cancellationToken);

                DateTime.TryParse(inboundMail.Headers.SingleOrDefault(w => w.Key == "Date").Value, CultureInfo.InvariantCulture, out DateTime date);

                var model = new EmailDocument(Guid.NewGuid().ToString())
                {
                    Subject = inboundMail.Subject,
                    Html = inboundMail.Html,
                    Text = inboundMail.Text,
                    From = new EmailAddress { Email = inboundMail.From.Email, Name = inboundMail.From.Name },
                    To = inboundMail.To.Select(s => new EmailAddress { Email = s.Email, Name = s.Name }).ToList(),
                    Cc = inboundMail.Cc.Select(s => new EmailAddress { Email = s.Email, Name = s.Name }).ToList(),
                    Date = date,
                    SenderIp = inboundMail.SenderIp
                };

                await repo.Upsert(model, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("SendgridSendEmail")]
        public async Task SendgridSendEmail(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "adm/send-email")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var inbound = await req.GetPublicBody<SendEmail>(cancellationToken);

                var apiKey = configuration.GetValue<string>("Sendgrid_Key");
                var strongGridClient = new Client(apiKey);

                var textContent = new MailContent("text/plain", inbound.Text);
                var htmlContent = new MailContent("text/html", inbound.Html);
                var from = new MailAddress(inbound.FromEmail, inbound.FromName);
                var to = new MailAddress(inbound.ToEmail, inbound.ToName);

                var personalization = new MailPersonalization
                {
                    From = from,
                    To = [to],
                    Subject = inbound.Subject,
                };

                var mailSettings = new MailSettings
                {
                    SandboxModeEnabled = true,
                    Footer = new FooterSettings
                    {
                        Enabled = true,
                        HtmlContent = "<p>This email was sent with the help of the <b><a href=\"https://www.nuget.org/packages/StrongGrid/\">StrongGrid</a></b> library</p>",
                        TextContent = "This email was sent with the help of the StrongGrid library"
                    },
                    BypassListManagement = false,
                    BypassSpamManagement = true,
                    BypassBounceManagement = true,
                    BypassUnsubscribeManagement = true
                };

                var trackingSettings = new TrackingSettings
                {
                    ClickTracking = new ClickTrackingSettings
                    {
                        EnabledInHtmlContent = true,
                        EnabledInTextContent = true
                    },
                    OpenTracking = new OpenTrackingSettings { Enabled = true },
                    GoogleAnalytics = new GoogleAnalyticsSettings { Enabled = false },
                    SubscriptionTracking = new SubscriptionTrackingSettings { Enabled = false }
                };

                await strongGridClient.Mail.SendAsync([personalization], inbound.Subject, [textContent, htmlContent], from, [to],
                    mailSettings: mailSettings, trackingSettings: trackingSettings, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}