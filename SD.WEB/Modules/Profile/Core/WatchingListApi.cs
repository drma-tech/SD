namespace SD.WEB.Modules.Profile.Core;

public class WatchingListApi(IHttpClientFactory factory) : ApiCosmos<WatchingList>(factory, ApiType.Authenticated, "watchinglist", ApiContext.Default.WatchingList)
{
    public async Task<WatchingList?> Get(ComponentActions<WatchingList?>? actions, CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.Get, false, actions, cancellationToken);
    }

    public async Task<WatchingList?> Add(MediaType? mediaType, WatchingList? obj, WatchingListItem? item, AccountProduct? product, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(item);
        SubscriptionHelper.ValidateWatching(product, (obj?.Items(mediaType).Count ?? 0) + 1);

        return await PostAsync(Endpoint.Add(mediaType), item, ApiContext.Default.WatchingListItem, cancellationToken);
    }

    public async Task<WatchingList?> Remove(MediaType? mediaType, string? collectionId, string? tmdbId = "null", CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(collectionId);

        return await PostAsync(Endpoint.Remove(mediaType, collectionId, tmdbId ?? "null"), null, ApiContext.Default.WatchingList, cancellationToken);
    }

    public async Task<WatchingList?> Sync(MediaType? mediaType, WatchingList? obj, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync(Endpoint.Sync(mediaType), obj, ApiContext.Default.WatchingList, cancellationToken);
    }

    private struct Endpoint
    {
        public const string Get = "watchinglist/get";

        public static string Add(MediaType? type)
        {
            return $"watchinglist/add/{type}";
        }

        public static string Remove(MediaType? type, string collectionId, string tmdbId)
        {
            return $"watchinglist/remove/{type}/{collectionId}/{tmdbId}";
        }

        public static string Sync(MediaType? type)
        {
            return $"watchinglist/sync/{type}";
        }
    }
}