using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Functions
{
    public class WishListFunction(CosmosRepository repo)
    {
        [Function("WishListGet")]
        public async Task<HttpResponseData?> WishListGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/wishlist/get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];
                WishList? doc;

                if (string.IsNullOrEmpty(id))
                {
                    var userId = req.GetUserId();
                    doc = await repo.Get<WishList>(DocumentType.WishList, userId, cancellationToken);
                }
                else
                {
                    doc = await repo.Get<WishList>(DocumentType.WishList, id, cancellationToken);
                }

                return await req.CreateResponse(doc, ttlCache.one_day, doc?.ETag, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("WishListAdd")]
        public async Task<WishList?> WishListAdd(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "wishlist/add/{type}")] HttpRequestData req,
            string type, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<WishList>(DocumentType.WishList, userId, cancellationToken);
                var newItem = await req.GetPublicBody<WishListItem>(cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(userId);
                }

                obj.AddItem(Enum.Parse<MediaType>(type), newItem);

                return await repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("WishListRemove")]
        public async Task<WishList?> WishListRemove(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "wishlist/remove/{type}/{id}")] HttpRequestData req,
            string type, string id, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<WishList>(DocumentType.WishList, userId, cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(userId);
                }

                obj.RemoveItem(Enum.Parse<MediaType>(type), id);

                return await repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }
    }
}