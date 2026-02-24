using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.Shared.Models.Auth;
using System.Net;

namespace SD.API.Functions;

public class LoginFunction(CosmosRepository repo, IHttpClientFactory factory)
{
    [Function("LoginGet")]
    public async Task<AuthLogin?> LoginGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "login/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(cancellationToken);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("unauthenticated user");

            return await repo.Get<AuthLogin>(DocumentType.Login, userId, cancellationToken);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("LoginAdd")]
    public async Task LoginAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "login/add")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var platform = req.GetQueryParameters()["platform"] ?? "webapp";
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
                    Accesses = [new Access { Date = now, Platform = platform, Ip = ip }]
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
                    .Union([new Access { Date = now, Platform = platform, Ip = ip }]).ToArray();

                await repo.UpsertItemAsync(login, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }

    [Function("Test")]
    public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/test")] HttpRequestData req)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.WriteString("OK");
        return response;
    }

    [Function("Country")]
    public async Task<string?> Country([HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/country")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var ip = req.GetUserIP(false);
            if (ip.Empty()) return null;
            if (ip == "127.0.0.1") return null;

            var client = factory.CreateClient("ipinfo");

            var result = await client.GetValueAsync($"https://ipinfo.io/{ip}/country", cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            return null;
        }
    }
}