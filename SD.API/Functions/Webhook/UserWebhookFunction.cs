using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.API.Core.Models;
using SD.API.Functions.Auth;
using Svix.Exceptions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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

    [Function("UserJotSignedIdentity")]
    public async Task<HttpResponseData> CreateSignedIdentity(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/userjot/identity")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync();
        var userEmail = await req.GetUserEmailAsync();
        var projectId = ApiStartup.Configurations.UserJot?.ProjectId;
        var secret = ApiStartup.Configurations.UserJot?.Secret;

        if (string.IsNullOrWhiteSpace(projectId) || string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException("UserJot signed identity is not configured.");
        }

        var now = DateTimeOffset.UtcNow;
        var payload = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["sub"] = userId,
            ["iss"] = projectId,
            ["aud"] = "userjot",
            ["iat"] = now.ToUnixTimeSeconds(),
            ["exp"] = now.AddMinutes(10).ToUnixTimeSeconds(),
        };

        payload["email"] = userEmail;

        var encodedHeader = ExtensionMethods.SimpleEncrypt(new { alg = "HS256", typ = "JWT" });
        var encodedPayload = ExtensionMethods.SimpleEncrypt(payload);
        var signingInput = $"{encodedHeader}.{encodedPayload}";
        var signature = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), Encoding.ASCII.GetBytes(signingInput));
        var token = $"{signingInput}.{ExtensionMethods.SimpleEncrypt(signature)}";

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Cache-Control", "no-store");
        response.Headers.Add("Pragma", "no-cache");
        await response.WriteAsJsonAsync(new { token }, cancellationToken);
        return response;
    }
}