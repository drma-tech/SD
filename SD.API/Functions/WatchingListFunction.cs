using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Functions
{
    public class WatchingListFunction(CosmosRepository repo)
    {
        [Function("WatchingListGet")]
        public async Task<HttpResponseData?> WatchingListGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/watchinglist/get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];
                WatchingList? doc;

                if (string.IsNullOrEmpty(id))
                {
                    var userId = req.GetUserId();
                    if (userId.Empty()) throw new InvalidOperationException("GetUserId null");

                    doc = await repo.Get<WatchingList>(DocumentType.WatchingList, userId, cancellationToken);
                }
                else
                {
                    doc = await repo.Get<WatchingList>(DocumentType.WatchingList, id, cancellationToken);
                }

                return await req.CreateResponse(doc, ttlCache.one_day, doc?.ETag, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("WatchingListAdd")]
        public async Task<WatchingList?> WatchingListAdd(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "watchinglist/add/{MediaType}")] HttpRequestData req,
            string MediaType, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<WatchingList>(DocumentType.WatchingList, userId, cancellationToken);
                var newItem = await req.GetPublicBody<WatchingListItem>(cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(userId);
                }

                obj.AddItem(Enum.Parse<MediaType>(MediaType), newItem);

                return await repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("WatchingListRemove")]
        public async Task<WatchingList?> WatchingListRemove(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "watchinglist/remove/{MediaType}/{CollectionId}/{TmdbId}")] HttpRequestData req,
            string MediaType, string CollectionId, string TmdbId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<WatchingList>(DocumentType.WatchingList, userId, cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(userId);
                }

                obj.RemoveItem(Enum.Parse<MediaType>(MediaType), CollectionId, TmdbId == "null" ? null : TmdbId);

                return await repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("WatchingListSync")]
        public async Task<WatchingList?> WatchingListSync(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "watchinglist/sync/{MediaType}")] HttpRequestData req,
            string MediaType, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<WatchingList>(DocumentType.WatchingList, userId, cancellationToken);
                var newItem = await req.GetPublicBody<WatchingList>(cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(userId);
                }

                var type = Enum.Parse<MediaType>(MediaType);

                if (type == Shared.Enums.MediaType.movie)
                {
                    foreach (var item in newItem.Movies)
                    {
                        obj.AddItem(Shared.Enums.MediaType.movie, item);
                    }
                    obj.MovieSyncDate = DateTime.Now;
                }
                else
                {
                    foreach (var item in newItem.Shows)
                    {
                        obj.AddItem(Shared.Enums.MediaType.tv, item);
                    }
                    obj.ShowSyncDate = DateTime.Now;
                }

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