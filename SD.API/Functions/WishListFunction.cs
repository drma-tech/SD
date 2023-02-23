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

        [Function("WishListGet")]
        public async Task<WishList?> Get(
            [HttpTrigger(AuthorizationLevel.Function, Method.GET, Route = "WishList/Get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

                return await _repo.Get<WishList>(DocumentType.WishList + ":" + userId, new PartitionKey(userId), cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("WishListAdd")]
        public async Task<WishList?> Add(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WishList/Add/{MediaType}")] HttpRequestData req,
            string MediaType, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

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

                obj.AddItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), newItem);

                return await _repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("WishListRemove")]
        public async Task<WishList?> Remove(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WishList/Remove/{MediaType}/{TmdbId}")] HttpRequestData req,
            string MediaType, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

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

                obj.RemoveItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), TmdbId);

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