using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Caching.Distributed;
using SD.API.Core.Auth;
using SD.Shared.Models.Auth;
using SD.Shared.Models.ZeptoMail;
using Supabase.Gotrue;
using System.Net;
using System.Text.Json;

namespace SD.API.Functions;

public class LoginFunction(CosmosRepository repo, IDistributedCache cache)
{
    [Function("LoginGet")]
    public async Task<AuthLogin?> LoginGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "login/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);
        if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("unauthenticated user");

        return await repo.Get<AuthLogin>(DocumentType.Login, userId, cancellationToken);
    }

    [Function("LoginAdd")]
    public async Task LoginAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "login/add")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var platform = req.GetQueryParameters()["platform"] ?? "error";
        var country = req.GetQueryParameters()["country"] ?? "error";
        var ip = req.GetUserIP(true);
        var userId = await req.GetUserIdAsync(cancellationToken);
        if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("unauthenticated user");
        var login = await repo.Get<AuthLogin>(DocumentType.Login, userId, cancellationToken);
        var now = DateTimeOffset.UtcNow;

        if (login == null)
        {
            var newLogin = new AuthLogin
            {
                UserId = userId,
                Accesses = [new Access { Date = now, Platform = platform, Ip = ip, Country = country.ToLower() }]
            };
            newLogin.Initialize(userId);

            await repo.UpsertItemAsync(newLogin, cancellationToken);
        }
        else
        {
            var minInterval = TimeSpan.FromHours(1);
            var lastAccess = login.Accesses.OrderByDescending(a => a.Date).FirstOrDefault();

            if (lastAccess != null && now - lastAccess.Date < minInterval)
            {
                return;
            }

            var cutoff = DateTimeOffset.UtcNow.AddMonths(-6); //Keep access history for the last 6 months only.

            login.Accesses = login.Accesses
                .Where(a => a.Date >= cutoff)
                .Union([new Access { Date = now, Platform = platform, Ip = ip, Country = country.ToLower() }])
                .Take(100)
                .ToArray();

            await repo.UpsertItemAsync(login, cancellationToken);
        }
    }

    //[Function("LoginFix")]
    //public async Task LoginFix(
    //   [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "login/fix")] HttpRequestData req, CancellationToken cancellationToken)
    //{
    //    var logins = await repo.ListAll<AuthLogin>(DocumentType.Login, cancellationToken);
    //    var client = factory.CreateClient("ipinfo");

    //    foreach (var login in logins)
    //    {
    //        foreach (var access in login.Accesses)
    //        {
    //            var ip = access.Ip?.Split(":")[0];
    //            if (ip.NotEmpty() && ip != "127.0.0.1")
    //            {
    //                var result = await client.GetStringAsync($"https://ipinfo.io/{ip}/country", cancellationToken);
    //                access.Country = result?.Trim()?.ToLower();
    //            }
    //        }

    //        await repo.UpsertItemAsync(login, cancellationToken);
    //    }
    //}

    [Function("LoginEmailAuth")]
    public async Task LoginEmailAuth(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/login/email")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var email = req.GetQueryParameters()["email"];
        var reference = req.GetQueryParameters()["reference"];

        if (email.Empty()) return;
        if (reference.Empty()) return;

        //generate a magic link and return the OTP to the client

        var client = new Supabase.Client(ApiStartup.Configurations.SupabaseAuth!.Url!, ApiStartup.Configurations.SupabaseAuth.Key);
        var admin = client.AdminAuth(ApiStartup.Configurations.SupabaseAuth!.ServiceKey!);

        var options = new GenerateLinkOptions(GenerateLinkOptions.LinkType.MagicLink, email);

        var response = await admin.GenerateLink(options);

        //send email using zeptomail

        var zepto = new ZeptoMailClient(ApiStartup.Configurations.ZeptoMail!.ApiKey!);

        await zepto.SendOtpEmail(email, reference, response?.EmailOtp, cancellationToken);
    }

    [Function("LoginEmailWebHook")]
    public async Task<HttpResponseData> LoginEmailWebHook(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/login/email/webhook")] HttpRequestData req, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(req.Body, leaveOpen: true);

        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        var signature = req.Headers.GetValues("producer-signature").FirstOrDefault();

        var valid = ZeptoMailVerify.VerifyZeptoWebhook(rawBody, signature!, ApiStartup.Configurations.ZeptoMail!.SecrectWebhook!);

        if (!valid)
        {
            return await req.CreateResponse(HttpStatusCode.Unauthorized, "invalid webhook signature");
        }

        var app = req.Headers.GetValues("app").FirstOrDefault();

        if (app != "sd")
        {
            return await req.CreateResponse(HttpStatusCode.NotAcceptable, $"webhook ignored -> app={app ?? "null"}");
        }

        var body = JsonSerializer.Deserialize<ZeptoMailWebHook>(rawBody);

        var eventMessage = body?.event_message?.FirstOrDefault();
        var eventData = eventMessage?.event_data?.FirstOrDefault();

        var reference = eventMessage?.email_info?.client_reference;
        var message = eventData?.details?.FirstOrDefault()?.diagnostic_message;
        var bytes = JsonSerializer.SerializeToUtf8Bytes(message);

        await cache.SetAsync($"login:{reference}", bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) }, cancellationToken);

        return await req.CreateResponse(HttpStatusCode.OK, "webhook received");
    }

    [Function("LoginEmailStatus")]
    public async Task<string?> LoginEmailStatus(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/login/status")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var reference = req.GetQueryParameters()["reference"];
        if (reference.Empty()) return null;

        var cachedBytes = await cache.GetAsync($"login:{reference}", cancellationToken);

        if (cachedBytes is { Length: > 0 })
        {
            return JsonSerializer.Deserialize<string>(cachedBytes);
        }
        else
        {
            return null;
        }
    }

    [Function("Test")]
    public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/test")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteString("OK");
        return response;
    }
}
