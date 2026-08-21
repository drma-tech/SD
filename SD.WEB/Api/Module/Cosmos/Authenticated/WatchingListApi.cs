using SD.WEB.Api.Core;

namespace SD.WEB.Api.Module.Cosmos.Authenticated;

public class WatchingListApi(IHttpClientFactory factory) : ApiCosmos<WatchingList>(factory, ApiType.Authenticated, "watchinglist", [], ApiContext.Default.WatchingList)
{
    public async Task<WatchingList?> Get(RenderControlState<WatchingList>? actions, CancellationToken cancellationToken)
    {
        if (!AppStateStatic.IsAuthenticated) return default;

        return await GetAsync("watchinglist/get", setNewVersion: false, actions, cancellationToken);
    }

    public async Task<WatchingList?> Add(MediaType? mediaType, WatchingList? obj, WatchingListItem? item, AccountProduct? product, CancellationToken cancellationToken)
    {
        if (!mediaType.HasValue)
        {
            throw new ArgumentNullException(nameof(mediaType));
        }

        ArgumentNullException.ThrowIfNull(item);
        SubscriptionHelper.ValidateWatching(product, (obj?.Items(mediaType).Count ?? 0) + 1);

        return await PostAsync($"watchinglist/add/{mediaType}", item, ApiContext.Default.WatchingListItem, state: null, cancellationToken);
    }

    public async Task<WatchingList?> Remove(MediaType? mediaType, string? collectionId, string? tmdbId = "null", CancellationToken cancellationToken = default)
    {
        if (!mediaType.HasValue)
        {
            throw new ArgumentNullException(nameof(mediaType));
        }

        ArgumentNullException.ThrowIfNull(collectionId);

        return await PostAsync($"watchinglist/remove/{mediaType}/{collectionId}/{tmdbId}", null, ApiContext.Default.WatchingList, state: null, cancellationToken);
    }

    public async Task<WatchingList?> Sync(MediaType? mediaType, WatchingList? obj, RenderControlState<WatchingList>? state, CancellationToken cancellationToken)
    {
        if (!mediaType.HasValue)
        {
            throw new ArgumentNullException(nameof(mediaType));
        }

        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync($"watchinglist/sync/{mediaType}", obj, ApiContext.Default.WatchingList, state, cancellationToken);
    }
}