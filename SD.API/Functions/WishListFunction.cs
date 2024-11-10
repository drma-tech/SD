using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Functions
{
    public class WishListFunction(CosmosRepository repo)
    {
        //[OpenApiOperation("WishListGet", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(WishList))]
        [Function("WishListGet")]
        public async Task<WishList?> WishListGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/wishlist/get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];

                if (string.IsNullOrEmpty(id))
                {
                    var userId = req.GetUserId();
                    return await repo.Get<WishList>(DocumentType.WishList, userId, cancellationToken);
                }
                else
                {
                    return await repo.Get<WishList>(DocumentType.WishList, id, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        //[OpenApiOperation("WishListAdd", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(WishList))]
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
                else
                {
                    obj.Update();
                }

                obj.AddItem((MediaType)Enum.Parse(typeof(MediaType), type), newItem);

                return await repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        //[OpenApiOperation("WishListRemove", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(WishList))]
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
                else
                {
                    obj.Update();
                }

                obj.RemoveItem((MediaType)Enum.Parse(typeof(MediaType), type), id);

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