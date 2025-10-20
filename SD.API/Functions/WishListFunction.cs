using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Functions;

public class WishListFunction(CosmosRepository repo)
{
    [Function("WishListGet")]
    public async Task<HttpResponseData?> WishListGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/wishlist/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var id = req.GetQueryParameters()["id"];
            WishList? doc;

            if (string.IsNullOrEmpty(id))
            {
                var userId = await req.GetUserIdAsync(cancellationToken);
                if (userId.Empty()) throw new InvalidOperationException("GetUserId null");

                doc = await repo.Get<WishList>(DocumentType.WishList, userId, cancellationToken);
            }
            else
            {
                doc = await repo.Get<WishList>(DocumentType.WishList, id, cancellationToken);
            }

            return await req.CreateResponse(doc, TtlCache.OneDay, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("WishListAdd")]
    public async Task<WishList?> WishListAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "wishlist/add/{type}")] HttpRequestData req, string type, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(cancellationToken);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<WishList>(DocumentType.WishList, userId, cancellationToken);
            var newItem = await req.GetPublicBody<WishListItem>(cancellationToken);

            if (obj == null)
            {
                obj = new WishList();

                obj.Initialize(userId);
            }

            obj.AddItem(Enum.Parse<MediaType>(type), newItem);

            return await repo.UpsertItemAsync(obj, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("WishListRemove")]
    public async Task<WishList?> WishListRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "wishlist/remove/{type}/{id}")] HttpRequestData req, string type, string id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(cancellationToken);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<WishList>(DocumentType.WishList, userId, cancellationToken);

            if (obj == null)
            {
                obj = new WishList();

                obj.Initialize(userId);
            }

            obj.RemoveItem(Enum.Parse<MediaType>(type), id);

            return await repo.UpsertItemAsync(obj, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }
}