using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SD.API.Core.Auth;
using SD.Shared.Core.Types;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Blocked;

namespace SD.API.Functions;

public class PrincipalFunction(CosmosMainRepository repo, CosmosCacheRepository repoCache)
{
    [Function("PrincipalGet")]
    public async Task<HttpResponseData?> PrincipalGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "principal/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);

        var model = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken);

        return await req.CreateResponse(model, TtlCache.OneDay, cancellationToken);
    }

    //[Function("PrincipalGetAll")]
    //public async Task<HttpResponseData?> PrincipalGetAll(
    //   [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "principal/get-all")] HttpRequestData req, CancellationToken cancellationToken)
    //{
    //    var data = await repo.ListAll<AuthPrincipal>(DocumentType.Principal, cancellationToken);

    //    return await req.CreateResponse(data, TtlCache.OneDay, cancellationToken);
    //}

    //[Function("PrincipalSyncAll")]
    //public async Task PrincipalSyncAll(
    //   [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "principal/sync")] HttpRequestData req, CancellationToken cancellationToken)
    //{
    //    var data = await repo.ListAll<AuthPrincipal>(DocumentType.Principal, cancellationToken);

    //    foreach (var item in data)
    //    {
    //        item._tsCreated ??= item._ts;

    //        await repo.UpsertItemAsync(item, cancellationToken);
    //    }
    //}

    [Function("PrincipalAdd")]
    public async Task<AuthPrincipal?> PrincipalAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "principal/add")] HttpRequestData req, CancellationToken cancellationToken)
    {
        //note: its called once per user (first access)

        var userId = await req.GetUserIdAsync(cancellationToken);
        var body = await req.GetBody<AuthPrincipal>(cancellationToken);
        var platform = req.GetQueryParameters()["platform"];
        var country = req.GetQueryParameters()["country"];

        await req.ValidateUser(body.UserId, cancellationToken);

        //check if user ip is blocked for insert
        var ip = req.GetUserIP(includePort: false) ?? throw new UnhandledException("Failed to retrieve IP");
        var blockedIp = await repoCache.ReadItemAsync<DataBlockedCache>(new CacheIdentity($"block-{ip}"), cancellationToken);
        if (blockedIp?.Data != null)
        {
            blockedIp.Data.Quantity++;
            await repoCache.UpsertItemAsync(blockedIp);

            if (blockedIp.Data?.Quantity > 2)
            {
                //todo: create a mechanism to increase block time if user persist on this action (first = block one hour, second = block 24 hours)
                req.LogWarning($"PrincipalAdd blocked IP {ip}");
                throw new NotificationException("You've reached the limit for creating profiles. Please try again later.");
            }
        }
        else
        {
            _ = repoCache.CreateItemAsync(new DataBlockedCache($"block-{ip}", new DataBlocked()));
        }

        foreach (var item in body.Events.Where(w => w.Ip.Empty()))
        {
            item.Ip = ip;
        }

        var zepto = new ZeptoMailClient(ApiStartup.Configurations.ZeptoMail!.JobApiKey!);
        if (body.Email.NotEmpty()) _ = zepto.SendWelcomeEmail(body.Email, userId, cancellationToken);

        var principal = new AuthPrincipal(userId)
        {
            AuthProviders = body.AuthProviders,
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

    [Function("PrincipalUpdate")]
    public async Task<AuthPrincipal?> PrincipalUpdate(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Put, Route = "principal/update")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);
        var body = await req.GetBody<AuthPrincipal>(cancellationToken);

        await req.ValidateUser(body.UserId, cancellationToken);

        var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken);

        principal!.AuthProviders = body.AuthProviders;

        return await repo.UpsertItemAsync(principal);
    }

    [Function("PrincipalEvent")]
    public async Task<AuthPrincipal> PrincipalEvent(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "principal/event")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);
        var ip = req.GetUserIP(includePort: true);

        var principal = await repo.ReadItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId), cancellationToken) ?? throw new UnhandledException("Client null");

        var app = req.GetQueryParameters()["app"];
        var msg = req.GetQueryParameters()["msg"];

        principal.Events.Add(new Event(app, msg, ip));

        return await repo.UpsertItemAsync(principal);
    }

    [Function("PrincipalRemove")]
    public async Task PrincipalRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Delete, Route = "principal/remove")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);

        await repo.DeleteItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, userId));
        await repo.DeleteItemAsync<AuthLogin>(new MainIdentity(MainType.Login, userId));
        await repo.DeleteItemAsync<MyProviders>(new MainIdentity(MainType.MyProvider, userId));
        await repo.DeleteItemAsync<WatchingList>(new MainIdentity(MainType.WatchingList, userId));
        await repo.DeleteItemAsync<WishList>(new MainIdentity(MainType.WishList, userId));
    }

    //[Function("PrincipalMigrate")]
    //public async Task PrincipalMigrate(
    //    [HttpTrigger(AuthorizationLevel.Anonymous, Method.Put, Route = "principal/migrate/{oldId}/{newId}")] HttpRequestData req, string oldId, string newId, CancellationToken cancellationToken)
    //{
    //    var myPrincipal = await repo.Get<AuthPrincipal>(DocumentType.Principal, oldId, cancellationToken);
    //    if (myPrincipal != null)
    //    {
    //        var model = myPrincipal.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myPrincipal, cancellationToken);
    //    }

    //    var myLogins = await repo.Get<AuthLogin>(DocumentType.Login, oldId, cancellationToken);
    //    if (myLogins != null)
    //    {
    //        var model = myLogins.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myLogins, cancellationToken);
    //    }

    //    var myProviders = await repo.Get<MyProviders>(DocumentType.MyProvider, oldId, cancellationToken);
    //    if (myProviders != null)
    //    {
    //        var model = myProviders.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myProviders, cancellationToken);
    //    }

    //    var mySuggestions = await repo.Get<MySuggestions>(DocumentType.MySuggestions, oldId, cancellationToken);
    //    if (mySuggestions != null)
    //    {
    //        var model = mySuggestions.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(mySuggestions, cancellationToken);
    //    }

    //    var myWatched = await repo.Get<WatchedList>(DocumentType.WatchedList, oldId, cancellationToken);
    //    if (myWatched != null)
    //    {
    //        var model = myWatched.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myWatched, cancellationToken);
    //    }

    //    var myWatching = await repo.Get<WatchingList>(DocumentType.WatchingList, oldId, cancellationToken);
    //    if (myWatching != null)
    //    {
    //        var model = myWatching.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myWatching, cancellationToken);
    //    }

    //    var myWish = await repo.Get<WishList>(DocumentType.WishList, oldId, cancellationToken);
    //    if (myWish != null)
    //    {
    //        var model = myWish.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myWish, cancellationToken);
    //    }
    //}
}
