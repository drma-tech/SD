using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;

namespace SD.API.Functions
{
    public class WatchingListFunction
    {
        private readonly IRepository _repo;

        public WatchingListFunction(IRepository repo)
        {
            _repo = repo;
        }

        [Function("WatchingListGet")]
        public async Task<HttpResponseData> Get(
            [HttpTrigger(AuthorizationLevel.Function, Method.GET, Route = "WatchingList/Get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                return await req.ProcessObject(result, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("WatchingListAdd")]
        public async Task<HttpResponseData> Add(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WatchingList/Add/{MediaType}")] HttpRequestData req,
            string MediaType, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);
                var newItem = await req.GetPublicBody<WatchingListItem>(cancellationToken);

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

        [Function("WatchingListRemove")]
        public async Task<HttpResponseData> Remove(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WatchingList/Remove/{MediaType}/{CollectionId}/{TmdbId}")] HttpRequestData req,
            string MediaType, string CollectionId, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.SetIds(req.GetUserId(), req.GetUserId());
                }
                else
                {
                    obj.Update();
                }

                obj.RemoveItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), CollectionId, TmdbId == "null" ? null : TmdbId);

                obj = await _repo.Upsert(obj, cancellationToken);

                return await req.ProcessObject(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("WatchingListSync")]
        public async Task<HttpResponseData> Sync(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WatchingList/Sync/{MediaType}")] HttpRequestData req,
            string MediaType, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.SetIds(req.GetUserId(), req.GetUserId());
                }
                else
                {
                    obj.Update();
                }

                var type = (MediaType)Enum.Parse(typeof(MediaType), MediaType);

                if (type == Shared.Enums.MediaType.movie)
                {
                    obj.MovieSyncDate = DateTime.Now;
                }
                else
                {
                    obj.ShowSyncDate = DateTime.Now;
                }

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