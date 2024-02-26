using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.Shared.Models.Support;
using StrongGrid;
using StrongGrid.Models;

namespace SD.API.Functions
{
    public class SendgridFunction(CosmosEmailRepository repo)
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
                StrongGrid.Models.Webhooks.InboundEmail inboundMail = await parser.ParseInboundEmailWebhookAsync(req.Body, cancellationToken);

                var model = new EmailDocument(Guid.NewGuid().ToString(), inboundMail);

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

                var apiKey = "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";
                var strongGridClient = new Client(apiKey);

                var content = new MailContent("HTML", inbound.Html);
                var from = new MailAddress("support@streamingdiscovery.com", "Support SD");
                var to = new MailAddress("dhiogoacioli@gmail.com", "Dhiogo Acioli");

                var personalization = new MailPersonalization
                {
                    From = from,
                    To = [to],
                    Subject = "res tst 8"
                };

                await strongGridClient.Mail.SendAsync([personalization], "res tst 8", [content], from, [to], cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}

//// using SendGrid's C# Library
//// https://github.com/sendgrid/sendgrid-csharp
//using SendGrid;
//using SendGrid.Helpers.Mail;
//using System;
//using System.Threading.Tasks;

//namespace Example
//{
//    internal class Example
//    {
//        private static void Main()
//        {
//            Execute().Wait();
//        }

//        static async Task Execute()
//        {
//            var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
//            var client = new SendGridClient(apiKey);
//            var from = new EmailAddress("test@example.com", "Example User");
//            var subject = "Sending with SendGrid is Fun";
//            var to = new EmailAddress("test@example.com", "Example User");
//            var plainTextContent = "and easy to do anywhere, even with C#";
//            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
//            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
//            var response = await client.SendEmailAsync(msg);
//        }
//    }
//}