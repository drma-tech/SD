using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.Shared.Core.Types;

namespace SD.API.Functions.Auth;

public class WatchingListFunction(CosmosMainRepository repo)
{
    [Function("WatchingListGet")]
    public async Task<HttpResponseData?> WatchingListGet(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "watchinglist/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync();

        var doc = await repo.ReadItemAsync<WatchingList>(new MainIdentity(MainType.WatchingList, userId), cancellationToken);

        return await req.CreateResponse(doc, TtlCache.OneDay, cancellationToken);
    }

    [Function("WatchingListAdd")]
    public async Task<WatchingList?> WatchingListAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "watchinglist/add/{MediaType}")] HttpRequestData req,
        string mediaType, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync();

        var obj = await repo.ReadItemAsync<WatchingList>(new MainIdentity(MainType.WatchingList, userId), cancellationToken);
        var newItem = await req.GetBody<WatchingListItem>(cancellationToken);

        obj ??= new WatchingList(userId);

        obj.AddItem(mediaType.ParseToEnum<MediaType>(), newItem);

        return await repo.UpsertItemAsync(obj);
    }

    [Function("WatchingListRemove")]
    public async Task<WatchingList?> WatchingListRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "watchinglist/remove/{MediaType}/{CollectionId}/{TmdbId}")] HttpRequestData req,
        string mediaType, string collectionId, string tmdbId, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync();

        var obj = await repo.ReadItemAsync<WatchingList>(new MainIdentity(MainType.WatchingList, userId), cancellationToken);

        obj ??= new WatchingList(userId);

        obj.RemoveItem(mediaType.ParseToEnum<MediaType>(), collectionId, string.Equals(tmdbId, "null", StringComparison.OrdinalIgnoreCase) ? null : tmdbId);

        return await repo.UpsertItemAsync(obj);
    }

    [Function("WatchingListSync")]
    public async Task<WatchingList?> WatchingListSync(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "watchinglist/sync/{MediaType}")] HttpRequestData req,
        string mediaType, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync();

        var obj = await repo.ReadItemAsync<WatchingList>(new MainIdentity(MainType.WatchingList, userId), cancellationToken);
        var newItem = await req.GetBody<WatchingList>(cancellationToken);

        obj ??= new WatchingList(userId);

        var type = mediaType.ParseToEnum<MediaType>();

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

        return await repo.UpsertItemAsync(obj);
    }
}