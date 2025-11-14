using FirebaseAdmin.Auth;
using FirebaseAdmin.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SD.API.Core.Auth;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Blocked;

namespace SD.API.Functions;

public class PrincipalFunction(CosmosRepository repo, CosmosCacheRepository repoCache, ILogger<PrincipalFunction> logger, IHttpClientFactory factory)
{
    [Function("PrincipalGet")]
    public async Task<HttpResponseData?> PrincipalGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "principal/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var model = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken);

            return await req.CreateResponse(model, TtlCache.OneDay, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("PrincipalAdd")]
    public async Task<AuthPrincipal?> PrincipalAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "principal/add")] HttpRequestData req, CancellationToken cancellationToken)
    {
        //note: its called once per user (first access)

        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);
            var body = await req.GetBody<AuthPrincipal>(factory, cancellationToken);

            if (userId.Empty()) throw new InvalidOperationException("unauthenticated user");

            //check if user ip is blocked for insert
            var ip = req.GetUserIP(false) ?? throw new NotificationException("Failed to retrieve IP");
            var blockedIp = await repoCache.Get<DataBlocked>($"block-{ip}", cancellationToken);
            if (blockedIp?.Data != null)
            {
                blockedIp.Data.Quantity++;
                await repoCache.UpsertItemAsync(blockedIp, cancellationToken);

                if (blockedIp.Data?.Quantity > 2)
                {
                    //todo: create a mechanism to increase block time if user persist on this action (first = block one hour, second = block 24 hours)
                    logger.LogWarning("PrincipalAdd blocked IP {IP}", ip);
                    throw new NotificationException("You've reached the limit for creating profiles. Please try again later.");
                }
            }
            else
            {
                _ = repoCache.CreateItemAsync(new DataBlockedCache(new DataBlocked(), $"block-{ip}", TtlCache.OneWeek), cancellationToken);
            }

            var model = new AuthPrincipal
            {
                IdentityProvider = body.IdentityProvider,
                DisplayName = body.DisplayName,
                Email = body.Email,
                Events = body.Events
            };
            model.Initialize(userId);

            return await repo.CreateItemAsync(model, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("PrincipalEvent")]
    public async Task<AuthPrincipal> PrincipalEvent(
       [HttpTrigger(AuthorizationLevel.Anonymous, Method.Put, Route = "principal/event")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);

            var model = await repo.Get<AuthPrincipal>(DocumentType.Principal, userId, cancellationToken) ?? throw new UnhandledException("Client null");
            var msg = req.GetQueryParameters()["msg"];

            model.Events = model.Events.Union([new Event { Description = msg }]).ToArray();

            return await repo.UpsertItemAsync(model, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("PrincipalRemove")]
    public async Task PrincipalRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Delete, Route = "principal/remove")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);

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
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("PrincipalImport")]
    public async Task PrincipalImport(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "principal/import")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var users = await repo.ListAll<AuthPrincipal>(DocumentType.Principal, cancellationToken);

            foreach (var user in users)
            {
                var args = new UserRecordArgs()
                {
                    Uid = user.Id.Split(":")[1],
                    Email = user.Email,
                    DisplayName = user.DisplayName
                };

                await FirebaseAuth.DefaultInstance.CreateUserAsync(args, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("SubscribeToTopics")]
    public static async Task SubscribeToTopics(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "firebase/subscribe")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var token = req.GetQueryParameters()["token"];
            var platform = req.GetQueryParameters()["platform"];

            await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync([token], "global");
            await FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync([token], platform);

            var message = new Message()
            {
                Token = token,
                Data = new Dictionary<string, string>
                {
                    { "title", "Streaming Discovery" },
                    { "body", "Welcome to your personal streaming guide." }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }
}