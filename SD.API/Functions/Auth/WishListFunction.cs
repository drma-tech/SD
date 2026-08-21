using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.Shared.Core.Types;

namespace SD.API.Functions.Auth;

public class WishListFunction(CosmosMainRepository repo)
{
    [Function("WishListGet")]
    public async Task<HttpResponseData?> WishListGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "wishlist/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);

        var doc = await repo.ReadItemAsync<WishList>(new MainIdentity(MainType.WishList, userId), cancellationToken);

        return await req.CreateResponse(doc, TtlCache.OneDay, cancellationToken);
    }

    [Function("WishListAdd")]
    public async Task<WishList?> WishListAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "wishlist/add/{type}")] HttpRequestData req, string type, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);

        var obj = await repo.ReadItemAsync<WishList>(new MainIdentity(MainType.WishList, userId), cancellationToken);
        var newItem = await req.GetBody<WishListItem>(cancellationToken);

        obj ??= new WishList(userId);

        obj.AddItem(type.ParseToEnum<MediaType>(), newItem);

        return await repo.UpsertItemAsync(obj);
    }

    [Function("WishListRemove")]
    public async Task<WishList?> WishListRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "wishlist/remove/{type}/{id}")] HttpRequestData req, string type, string id, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);

        var obj = await repo.ReadItemAsync<WishList>(new MainIdentity(MainType.WishList, userId), cancellationToken);

        obj ??= new WishList(userId);

        obj.RemoveItem(type.ParseToEnum<MediaType>(), id);

        return await repo.UpsertItemAsync(obj);
    }
}