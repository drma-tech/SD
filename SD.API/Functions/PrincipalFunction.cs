using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SD.API.Core.Auth;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Blocked;

namespace SD.API.Functions;

public class PrincipalFunction(CosmosRepository repo, CosmosCacheRepository repoCache)
{
    [Function("PrincipalGet")]
    public async Task<HttpResponseData?> PrincipalGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "principal/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);
        if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

        var model = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken);

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

        if (userId.Empty()) throw new InvalidOperationException("unauthenticated user");

        //check if user ip is blocked for insert
        var ip = req.GetUserIP(false) ?? throw new UnhandledException("Failed to retrieve IP");
        var blockedIp = await repoCache.Get<DataBlocked>($"block-{ip}", cancellationToken);
        if (blockedIp?.Data != null)
        {
            blockedIp.Data.Quantity++;
            await repoCache.UpsertItemAsync(blockedIp, cancellationToken);

            if (blockedIp.Data?.Quantity > 2)
            {
                //todo: create a mechanism to increase block time if user persist on this action (first = block one hour, second = block 24 hours)
                req.LogWarning($"PrincipalAdd blocked IP {ip}");
                throw new NotificationException("You've reached the limit for creating profiles. Please try again later.");
            }
        }
        else
        {
            _ = repoCache.CreateItemAsync(new DataBlockedCache(new DataBlocked(), $"block-{ip}", TtlCache.OneWeek), cancellationToken);
        }

        foreach (var item in body.Events.Where(w => w.Ip.Empty()))
        {
            item.Ip = ip;
        }

        var principal = new AuthPrincipal
        {
            AuthProviders = body.AuthProviders,
            DisplayName = body.DisplayName,
            Email = body.Email,
            Events = body.Events
        };
        principal.Initialize(userId);

        return await repo.CreateItemAsync(principal, cancellationToken);
    }

    [Function("PrincipalUpdate")]
    public async Task<AuthPrincipal?> PrincipalUpdate(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Put, Route = "principal/update")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);
        var body = await req.GetBody<AuthPrincipal>(cancellationToken);

        if (userId.Empty()) throw new InvalidOperationException("unauthenticated user");

        var principal = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken);

        principal!.AuthProviders = body.AuthProviders;

        return await repo.UpsertItemAsync(principal, cancellationToken);
    }

    [Function("PrincipalEvent")]
    public async Task<AuthPrincipal> PrincipalEvent(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Put, Route = "principal/event")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);
        var ip = req.GetUserIP(true);

        var principal = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken) ?? throw new UnhandledException("Client null");

        var app = req.GetQueryParameters()["app"];
        var msg = req.GetQueryParameters()["msg"];

        principal.Events.Add(new Event(app, msg, ip));

        return await repo.UpsertItemAsync(principal, cancellationToken);
    }

    [Function("PrincipalRemove")]
    public async Task PrincipalRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Delete, Route = "principal/remove")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);

        var myPrincipal = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken);
        if (myPrincipal != null) await repo.Delete(myPrincipal, cancellationToken);

        var myLogins = await repo.Get<AuthLogin>(DocumentType.Login, userId, cancellationToken);
        if (myLogins != null) await repo.Delete(myLogins, cancellationToken);

        var myProviders = await repo.Get<MyProviders>(DocumentType.MyProvider, userId, cancellationToken);
        if (myProviders != null) await repo.Delete(myProviders, cancellationToken);

        var mySuggestions = await repo.Get<MySuggestions>(DocumentType.MySuggestions, userId, cancellationToken);
        if (mySuggestions != null) await repo.Delete(mySuggestions, cancellationToken);

        var myWatched = await repo.Get<WatchedList>(DocumentType.WatchedList, userId, cancellationToken);
        if (myWatched != null) await repo.Delete(myWatched, cancellationToken);

        var myWatching = await repo.Get<WatchingList>(DocumentType.WatchingList, userId, cancellationToken);
        if (myWatching != null) await repo.Delete(myWatching, cancellationToken);

        var myWish = await repo.Get<WishList>(DocumentType.WishList, userId, cancellationToken);
        if (myWish != null) await repo.Delete(myWish, cancellationToken);
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
