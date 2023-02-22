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
        public async Task<WatchingList?> Get(
            [HttpTrigger(AuthorizationLevel.Function, Method.GET, Route = "WatchingList/Get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                return await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("WatchingListAdd")]
        public async Task<WatchingList?> Add(
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

                    obj.Initialize(req.GetUserId());
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

        [Function("WatchingListRemove")]
        public async Task<WatchingList?> Remove(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WatchingList/Remove/{MediaType}/{CollectionId}/{TmdbId}")] HttpRequestData req,
            string MediaType, string CollectionId, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(req.GetUserId());
                }
                else
                {
                    obj.Update();
                }

                obj.RemoveItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), CollectionId, TmdbId == "null" ? null : TmdbId);

                return await _repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("WatchingListSync")]
        public async Task<WatchingList?> Sync(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WatchingList/Sync/{MediaType}")] HttpRequestData req,
            string MediaType, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _repo.Get<WatchingList>(DocumentType.WatchingList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(req.GetUserId());
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