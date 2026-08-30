using Clerk.BackendAPI;
using Clerk.BackendAPI.Models.Operations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.Shared.Core.Types;
using SD.Shared.Models.Auth;

namespace SD.API.Functions.Admin;

public class PrincipalFunction(CosmosMainRepository repo)
{
    private const string CloneFailed = "DeepClone failed";

    //[Function("PrincipalGetAll")]
    //public async Task<HttpResponseData?> PrincipalGetAll(
    //   [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "principal/get-all")] HttpRequestData req, CancellationToken cancellationToken)
    //{
    //    var data = await repo.ListAll<AuthPrincipal>(DocumentType.Principal, cancellationToken);

    //    return await req.CreateResponse(data, TtlCache.OneDay, cancellationToken);
    //}

    //[Function("PrincipalMigrate")]
    //public async Task PrincipalMigrate(
    //    [HttpTrigger(AuthorizationLevel.Anonymous, Method.Put, Route = "principal/migrate")] HttpRequestData req, CancellationToken cancellationToken)
    //{
    //    var principais = await repo.Query<AuthPrincipal>(MainType.Principal, predicate: null, transform: null, cancellationToken);
    //    var sdk = new ClerkBackendApi(bearerAuth: ApiStartup.Configurations.ClerkAuth!.SecretKey);

    //    foreach (var principal in principais)
    //    {
    //        var request = new CreateUserRequestBody()
    //        {
    //            FirstName = principal.DisplayName?.Split(" ").ElementAtIndex(0),
    //            LastName = principal.DisplayName?.Split(" ").ElementAtIndex(1),
    //            EmailAddress = [principal.Email!],
    //        };

    //        var user = await sdk.Users.CreateAsync(request);

    //        var clone = principal.DeepClone() ?? throw new NotificationException(CloneFailed);
    //        clone.ChangeIdentity(new MainIdentity(MainType.Principal, user.User!.Id));
    //        clone.UserId = user.User.Id;
    //        await repo.CreateItemAsync(clone);
    //        await repo.DeleteItemAsync<AuthPrincipal>(new MainIdentity(MainType.Principal, principal.Id));

    //        var myLogins = await repo.ReadItemAsync<AuthLogin>(new MainIdentity(MainType.Login, principal.Id), cancellationToken);
    //        if (myLogins != null)
    //        {
    //            var model = myLogins.DeepClone() ?? throw new NotificationException(CloneFailed);
    //            model.ChangeIdentity(new MainIdentity(MainType.Login, user.User.Id));
    //            clone.UserId = user.User.Id;
    //            await repo.CreateItemAsync(model);
    //            await repo.DeleteItemAsync<AuthLogin>(new MainIdentity(MainType.Login, principal.Id));
    //        }

    //        var myProviders = await repo.ReadItemAsync<MyProviders>(new MainIdentity(MainType.MyProvider, principal.Id), cancellationToken);
    //        if (myProviders != null)
    //        {
    //            var model = myProviders.DeepClone() ?? throw new NotificationException(CloneFailed);
    //            model.ChangeIdentity(new MainIdentity(MainType.MyProvider, user.User.Id));
    //            await repo.CreateItemAsync(model);
    //            await repo.DeleteItemAsync<MyProviders>(new MainIdentity(MainType.MyProvider, principal.Id));
    //        }

    //        var myWatching = await repo.ReadItemAsync<WatchingList>(new MainIdentity(MainType.WatchingList, principal.Id), cancellationToken);
    //        if (myWatching != null)
    //        {
    //            var model = myWatching.DeepClone() ?? throw new NotificationException(CloneFailed);
    //            model.ChangeIdentity(new MainIdentity(MainType.WatchingList, user.User.Id));
    //            await repo.CreateItemAsync(model);
    //            await repo.DeleteItemAsync<WatchingList>(new MainIdentity(MainType.WatchingList, principal.Id));
    //        }

    //        var myWish = await repo.ReadItemAsync<WishList>(new MainIdentity(MainType.WishList, principal.Id), cancellationToken);
    //        if (myWish != null)
    //        {
    //            var model = myWish.DeepClone() ?? throw new NotificationException(CloneFailed);
    //            model.ChangeIdentity(new MainIdentity(MainType.WishList, user.User.Id));
    //            await repo.CreateItemAsync(model);
    //            await repo.DeleteItemAsync<WishList>(new MainIdentity(MainType.WishList, principal.Id));
    //        }
    //    }
    //}
}