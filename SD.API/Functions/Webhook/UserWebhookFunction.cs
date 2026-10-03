using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Functions.Auth;
using Svix.Exceptions;
using System.Net;
using System.Text.Json;

namespace SD.API.Functions.Webhook;

public class UserWebhookFunction(CosmosMainRepository repo)
{
    [Function("PostClerkWebhook")]
    public async Task<HttpResponseData> PostClerkWebhook(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/clerk/webhook")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var json = await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);

        var signingSecret = ApiStartup.Configurations.ClerkAuth?.SigningSecret ?? throw new InvalidOperationException("Clerk Signing secret is not configured");

        // Headers do Svix/Clerk
        if (!req.Headers.TryGetValues("svix-id", out var idValues) ||
            !req.Headers.TryGetValues("svix-timestamp", out var timestampValues) ||
            !req.Headers.TryGetValues("svix-signature", out var signatureValues))
        {
            return await req.CreateResponse(HttpStatusCode.Unauthorized, "Missing Svix headers", cancellationToken);
        }

        var headers = new WebHeaderCollection { { "svix-id", idValues.First() }, { "svix-timestamp", timestampValues.First() }, { "svix-signature", signatureValues.First() } };

        try
        {
            var wh = new Svix.Webhook(signingSecret);
            wh.Verify(json, headers);
        }
        catch (WebhookVerificationException)
        {
            return await req.CreateResponse(HttpStatusCode.Unauthorized, "Invalid webhook signature", cancellationToken);
        }

        var obj = JsonSerializer.Deserialize<ClerkWebhook>(json);

        if (string.Equals(obj?.type, "user.deleted", StringComparison.OrdinalIgnoreCase))
        {
            if (obj?.data?.deleted == true)
            {
                await PrincipalHelper.DeleteUser(repo, obj.data.id, deleteClerk: false);
            }
        }

        return await req.CreateResponse(HttpStatusCode.OK, "webhook received", cancellationToken);
    }
}