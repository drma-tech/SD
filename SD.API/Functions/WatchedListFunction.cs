using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Functions
{
    public class WatchedListFunction(CosmosRepository repo)
    {
        [Function("WatchedListGet")]
        public async Task<HttpResponseData?> WatchedListGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/watchedlist/get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];
                WatchedList? doc;

                if (string.IsNullOrEmpty(id))
                {
                    var userId = req.GetUserId();
                    doc = await repo.Get<WatchedList>(DocumentType.WatchedList, userId, cancellationToken);
                }
                else
                {
                    doc = await repo.Get<WatchedList>(DocumentType.WatchedList, id, cancellationToken);
                }

                return await req.CreateResponse(doc, ttlCache.one_day, doc?.ETag, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("WatchedListAdd")]
        public async Task<WatchedList?> WatchedListAdd(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "watchedlist/add/{MediaType}/{TmdbId}")] HttpRequestData req,
            string MediaType, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<WatchedList>(DocumentType.WatchedList, userId, cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(userId);
                }

                var ids = TmdbId.Split(',');
                obj.AddItem(Enum.Parse<MediaType>(MediaType), new HashSet<string>(ids));

                return await repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("WatchedListRemove")]
        public async Task<WatchedList?> WatchedListRemove(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "watchedlist/remove/{MediaType}/{TmdbId}")] HttpRequestData req,
            string MediaType, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<WatchedList>(DocumentType.WatchedList, userId, cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(userId);
                }

                obj.RemoveItem(Enum.Parse<MediaType>(MediaType), TmdbId);

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