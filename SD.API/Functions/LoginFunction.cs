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
            var ip = req.GetUserIP();
            var userId = await req.GetUserIdAsync(cancellationToken);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("unauthenticated user");
            var login = await repo.Get<AuthLogin>(DocumentType.Login, userId, cancellationToken);

            if (login == null)
            {
                var newLogin = new AuthLogin
                {
                    UserId = userId,
                    Accesses = [new Access { Date = DateTimeOffset.UtcNow, Platform = platform, Ip = ip }]
                };
                newLogin.Initialize(userId);

                await repo.UpsertItemAsync(newLogin, cancellationToken);
            }
            else
            {
                login.Accesses = login.Accesses
                    .Union([new Access { Date = DateTimeOffset.UtcNow, Platform = platform, Ip = ip }]).ToArray();

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

    [Function("Logger")]
    public static async Task Logger([HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "public/logger")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var log = await req.GetPublicBody<LogModel>(cancellationToken);

            req.LogError(null, null, log);
        }
        catch (Exception)
        {
            req.LogError(null, null, null);
        }
    }

    [Function("Country")]
    public async Task<string?> Country([HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/country")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var ip = req.GetUserIP();
            if (ip == "127.0.0.1") ip = "8.8.8.8";
            var client = factory.CreateClient("ipinfo");

            var result = await client.GetValueAsync($"https://ipinfo.io/{ip}/country", cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            req.LogError(ex);
            throw;
        }
    }
}