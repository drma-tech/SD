using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.Shared.Models.Auth;
using System.Net;

namespace SD.API.Functions;

public class LoginFunction(CosmosRepository repo)
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
                Accesses = [new Access { Date = now, Platform = platform, Ip = ip, Country = country }]
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
                .Union([new Access { Date = now, Platform = platform, Ip = ip, Country = country }])
                .Take(100)
                .ToArray();

            await repo.UpsertItemAsync(login, cancellationToken);
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