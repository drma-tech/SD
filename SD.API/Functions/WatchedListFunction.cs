using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;

namespace SD.API.Functions
{
    public class WatchedListFunction
    {
        private readonly IRepository _repo;

        public WatchedListFunction(IRepository repo)
        {
            _repo = repo;
        }

        [Function("WatchedListGet")]
        public async Task<HttpResponseData> Get(
            [HttpTrigger(AuthorizationLevel.Function, Method.GET, Route = "WatchedList/Get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repo.Get<WatchedList>(DocumentType.WatchedList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                return await req.ProcessObject(result, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("WatchedListAdd")]
        public async Task<HttpResponseData> Add(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WatchedList/Add/{MediaType}/{TmdbId}")] HttpRequestData req,
            string MediaType, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _repo.Get<WatchedList>(DocumentType.WatchedList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(req.GetUserId());
                }
                else
                {
                    obj.Update();
                }

                var ids = TmdbId.Split(',');
                obj.AddItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), new HashSet<string>(ids));

                obj = await _repo.Upsert(obj, cancellationToken);

                return await req.ProcessObject(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("WatchedListRemove")]
        public async Task<HttpResponseData> Remove(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "WatchedList/Remove/{MediaType}/{TmdbId}")] HttpRequestData req,
            string MediaType, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _repo.Get<WatchedList>(DocumentType.WatchedList + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(req.GetUserId());
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