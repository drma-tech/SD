using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Functions;

public class WatchingListFunction(CosmosRepository repo, IHttpClientFactory factory)
{
    [Function("WatchingListGet")]
    public async Task<HttpResponseData?> WatchingListGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "watchinglist/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);
            if (userId.Empty()) throw new InvalidOperationException("GetUserId null");

            var doc = await repo.Get<WatchingList>(DocumentType.WatchingList, userId, cancellationToken);

            return await req.CreateResponse(doc, TtlCache.OneDay, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("WatchingListAdd")]
    public async Task<WatchingList?> WatchingListAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "watchinglist/add/{MediaType}")] HttpRequestData req,
        string mediaType, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<WatchingList>(DocumentType.WatchingList, userId, cancellationToken);
            var newItem = await req.GetPublicBody<WatchingListItem>(cancellationToken);

            if (obj == null)
            {
                obj = new WatchingList();

                obj.Initialize(userId);
            }

            obj.AddItem(Enum.Parse<MediaType>(mediaType), newItem);

            return await repo.UpsertItemAsync(obj, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("WatchingListRemove")]
    public async Task<WatchingList?> WatchingListRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "watchinglist/remove/{MediaType}/{CollectionId}/{TmdbId}")] HttpRequestData req,
        string mediaType, string collectionId, string tmdbId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<WatchingList>(DocumentType.WatchingList, userId, cancellationToken);

            if (obj == null)
            {
                obj = new WatchingList();

                obj.Initialize(userId);
            }

            obj.RemoveItem(Enum.Parse<MediaType>(mediaType), collectionId, tmdbId == "null" ? null : tmdbId);

            return await repo.UpsertItemAsync(obj, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("WatchingListSync")]
    public async Task<WatchingList?> WatchingListSync(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "watchinglist/sync/{MediaType}")] HttpRequestData req,
        string mediaType, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(factory, cancellationToken);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<WatchingList>(DocumentType.WatchingList, userId, cancellationToken);
            var newItem = await req.GetPublicBody<WatchingList>(cancellationToken);

            if (obj == null)
            {
                obj = new WatchingList();

                obj.Initialize(userId);
            }

            var type = Enum.Parse<MediaType>(mediaType);

            if (type == MediaType.movie)
            {
                foreach (var item in newItem.Movies) obj.AddItem(MediaType.movie, item);
                obj.MovieSyncDate = DateTime.Now;
            }
            else
            {
                foreach (var item in newItem.Shows) obj.AddItem(MediaType.tv, item);
                obj.ShowSyncDate = DateTime.Now;
            }

            return await repo.UpsertItemAsync(obj, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }
}