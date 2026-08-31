using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.Shared.Core.Types;
using SD.Shared.Models.Auth;

namespace SD.API.Functions.Auth;

public class PrincipalFunction(CosmosMainRepository repo, IHttpClientFactory factory)
{
    [Function("PrincipalGet")]
    public async Task<HttpResponseData?> PrincipalGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "principal/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync();

        var model = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken);

        return await req.CreateResponse(model, TtlCache.OneHour, cancellationToken);
    }

    [Function("PrincipalAdd")]
    public async Task<AuthPrincipal?> PrincipalAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "principal/add")] HttpRequestData req, CancellationToken cancellationToken)
    {
        //note: its called once per user (first access)

        var userId = await req.GetUserIdAsync();
        var body = await req.GetBody<AuthPrincipal>(cancellationToken);
        var platform = req.GetQueryParameters()["platform"];
        var country = req.GetQueryParameters()["country"];

        await req.ValidateUser(body.UserId, cancellationToken);

        var ip = req.GetUserIP(includePort: false) ?? throw new UnhandledException("Failed to retrieve IP");

        foreach (var item in body.Events.Where(w => w.Ip.Empty()))
        {
            item.Ip = ip;
        }

        var zepto = new ZeptoMailClient(factory, ApiStartup.Configurations.ZeptoMail!.JobApiKey!);
        if (body.Email.NotEmpty()) _ = zepto.SendWelcomeEmail(body.Email, userId, cancellationToken);

        var principal = new AuthPrincipal(userId)
        {
            DisplayName = body.DisplayName,
            Email = body.Email,
            Events = body.Events,
        };

        principal = await repo.CreateItemAsync(principal);

        if (platform.NotEmpty())
        {
            var newLogin = new AuthLogin(userId)
            {
                UserId = userId,
                Accesses = new HashSet<Access> { new() { Date = DateTimeOffset.UtcNow, Platform = platform, Ip = ip, Country = country } },
            };

            await repo.CreateItemAsync(newLogin);
        }

        return principal;
    }

    [Function("PrincipalEvent")]
    public async Task<AuthPrincipal> PrincipalEvent(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "principal/event")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync();
        var ip = req.GetUserIP(includePort: true);

        var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken) ?? throw new UnhandledException("Client null");

        var app = req.GetQueryParameters()["app"];
        var msg = req.GetQueryParameters()["msg"];

        principal.Events.Add(new Event(app, msg, ip));

        return await repo.UpsertItemAsync(principal);
    }

    [Function("PrincipalRemove")]
    public async Task PrincipalRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Delete, Route = "principal/remove")] HttpRequestData req)
    {
        var userId = await req.GetUserIdAsync();

        await repo.DeleteItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId));
        await repo.DeleteItemAsync<AuthLogin>(new MainIdentity(MainType.Login, userId));
        await repo.DeleteItemAsync<MyProviders>(new MainIdentity(MainType.MyProvider, userId));
        await repo.DeleteItemAsync<WatchingList>(new MainIdentity(MainType.WatchingList, userId));
        await repo.DeleteItemAsync<WishList>(new MainIdentity(MainType.WishList, userId));
    }
}