using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using SD.Shared.Models.Support;
using StrongGrid;

namespace SD.API.Functions
{
    public class SendgridFunction(EmailRepository repo, IConfiguration configuration)
    {
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
    }
}