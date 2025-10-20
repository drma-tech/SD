using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Functions;

public class WatchedListFunction(CosmosRepository repo)
{
    [Function("WatchedListGet")]
    public async Task<HttpResponseData?> WatchedListGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/watchedlist/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var id = req.GetQueryParameters()["id"];
            WatchedList? doc;

            if (string.IsNullOrEmpty(id))
            {
                var userId = await req.GetUserIdAsync(cancellationToken);
                if (userId.Empty()) throw new InvalidOperationException("GetUserId null");

                doc = await repo.Get<WatchedList>(DocumentType.WatchedList, userId, cancellationToken);
            }
            else
            {
                doc = await repo.Get<WatchedList>(DocumentType.WatchedList, id, cancellationToken);
            }

            return await req.CreateResponse(doc, TtlCache.OneDay, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("WatchedListAdd")]
    public async Task<WatchedList?> WatchedListAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "watchedlist/add/{MediaType}/{TmdbId}")] HttpRequestData req,
        string mediaType, string tmdbId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(cancellationToken);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<WatchedList>(DocumentType.WatchedList, userId, cancellationToken);

            if (obj == null)
            {
                obj = new WatchedList();

                obj.Initialize(userId);
            }

            var ids = tmdbId.Split(',');
            obj.AddItem(Enum.Parse<MediaType>(mediaType), [.. ids]);

            return await repo.UpsertItemAsync(obj, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("WatchedListRemove")]
    public async Task<WatchedList?> WatchedListRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "watchedlist/remove/{MediaType}/{TmdbId}")] HttpRequestData req,
        string mediaType, string tmdbId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await req.GetUserIdAsync(cancellationToken);
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<WatchedList>(DocumentType.WatchedList, userId, cancellationToken);

            if (obj == null)
            {
                obj = new WatchedList();

                obj.Initialize(userId);
            }

            obj.RemoveItem(Enum.Parse<MediaType>(mediaType), tmdbId);

            return await repo.UpsertItemAsync(obj, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }
}