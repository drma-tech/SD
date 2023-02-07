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
        public async Task<HttpResponseData> Get(
            [HttpTrigger(AuthorizationLevel.Function, Method.GET, Route = "WishList/Get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repo.Get<WishList>(DocumentType.WishList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                return await req.ProcessObject(result, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("WishListAdd")]
        public async Task<HttpResponseData> Add(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WishList/Add/{MediaType}")] HttpRequestData req,
            string MediaType, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _repo.Get<WishList>(DocumentType.WishList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);
                var newItem = await req.GetPublicBody<WishListItem>(cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.SetIds(req.GetUserId(), req.GetUserId());
                }
                else
                {
                    obj.Update();
                }

                obj.AddItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), newItem);

                obj = await _repo.Upsert(obj, cancellationToken);

                return await req.ProcessObject(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("WishListRemove")]
        public async Task<HttpResponseData> Remove(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WishList/Remove/{MediaType}/{TmdbId}")] HttpRequestData req,
            string MediaType, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _repo.Get<WishList>(DocumentType.WishList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.SetIds(req.GetUserId(), req.GetUserId());
                }
                else
                {
                    obj.Update();
                }

                obj.RemoveItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), TmdbId);

                obj = await _repo.Upsert(obj, cancellationToken);

                return await req.ProcessObject(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }
    }
}