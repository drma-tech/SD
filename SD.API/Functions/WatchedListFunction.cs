using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;

namespace SD.API.Functions
{
    public class WatchedListFunction(IRepository repo)
    {
        //[OpenApiOperation("WatchedListGet", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(WatchedList))]
        [Function("WatchedListGet")]
        public async Task<WatchedList?> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/watchedlist/get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];

                if (string.IsNullOrEmpty(id))
                {
                    var userId = req.GetUserId();
                    return await repo.Get<WatchedList>(DocumentType.WatchedList + ":" + userId, new PartitionKey(userId), cancellationToken);
                }
                else
                {
                    return await repo.Get<WatchedList>(DocumentType.WatchedList + ":" + id, new PartitionKey(id), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        //[OpenApiOperation("WatchedListAdd", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(WatchedList))]
        [Function("WatchedListAdd")]
        public async Task<WatchedList?> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "watchedlist/add/{MediaType}/{TmdbId}")] HttpRequestData req,
            string MediaType, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<WatchedList>(DocumentType.WatchedList + ":" + userId, new PartitionKey(userId), cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(userId);
                }
                else
                {
                    obj.Update();
                }

                var ids = TmdbId.Split(',');
                obj.AddItem((MediaType)Enum.Parse(typeof(MediaType), MediaType), new HashSet<string>(ids));

                return await repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        //[OpenApiOperation("WatchedListRemove", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(WatchedList))]
        [Function("WatchedListRemove")]
        public async Task<WatchedList?> Remove(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "watchedlist/remove/{MediaType}/{TmdbId}")] HttpRequestData req,
            string MediaType, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<WatchedList>(DocumentType.WatchedList + ":" + userId, new PartitionKey(userId), cancellationToken);

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

                return await repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}