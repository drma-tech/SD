using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;

namespace SD.API.Functions
{
    public class WishListFunction
    {
        private readonly IRepository _repo;

        public WishListFunction(IRepository repo)
        {
            _repo = repo;
        }

        //[OpenApiOperation("WishListGet", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(WishList))]
        [Function("WishListGet")]
        public async Task<WishList?> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "wishlist/get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                var id = req.GetQueryParameters()["id"];

                if (string.IsNullOrEmpty(id))
                    return await _repo.Get<WishList>(DocumentType.WishList + ":" + userId, new PartitionKey(userId), cancellationToken);
                else
                    return await _repo.Get<WishList>(DocumentType.WishList + ":" + id, new PartitionKey(id), cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        //[OpenApiOperation("WishListAdd", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(WishList))]
        [Function("WishListAdd")]
        public async Task<WishList?> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "wishlist/add/{type}")] HttpRequestData req,
            string type, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await _repo.Get<WishList>(DocumentType.WishList + ":" + userId, new PartitionKey(userId), cancellationToken);
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

                return await _repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        //[OpenApiOperation("WishListRemove", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(WishList))]
        [Function("WishListRemove")]
        public async Task<WishList?> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "wishlist/remove/{type}/{id}")] HttpRequestData req,
            string type, string id, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await _repo.Get<WishList>(DocumentType.WishList + ":" + userId, new PartitionKey(userId), cancellationToken);

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

                return await _repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}