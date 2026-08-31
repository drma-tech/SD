using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.Shared.Core.Types;
using SD.Shared.Models.Auth;

namespace SD.API.Functions.Auth;

public class LoginFunction(CosmosMainRepository repo)
{
    [Function("LoginGet")]
    public async Task<AuthLogin?> LoginGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "login/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync();

        return await repo.ReadItemAsync<AuthLogin>(new MainIdentity(MainType.Login, userId), cancellationToken);
    }

    [Function("LoginAdd")]
    public async Task LoginAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "login/add")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var platform = req.GetQueryParameters()["platform"] ?? "error";
        var country = req.GetQueryParameters()["country"] ?? "error";
        var ip = req.GetUserIP(includePort: true);
        var userId = await req.GetUserIdAsync();
        var login = await repo.ReadItemAsync<AuthLogin>(new MainIdentity(MainType.Login, userId), cancellationToken);
        var now = DateTimeOffset.UtcNow;

        if (login == null)
        {
            var newLogin = new AuthLogin(userId)
            {
                UserId = userId,
                Accesses = new HashSet<Access> { new() { Date = now, Platform = platform, Ip = ip, Country = country } },
            };

            await repo.CreateItemAsync(newLogin);
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

            login.Accesses = new HashSet<Access>([.. login.Accesses
                .Where(a => a.Date >= cutoff)
                .Union([new Access { Date = now, Platform = platform, Ip = ip, Country = country }])
                .Take(100)]);

            await repo.UpsertItemAsync(login);
        }
    }
}