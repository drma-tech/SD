namespace SD.WEB.Modules.Profile.Core;

public class WatchedListApi(IHttpClientFactory factory) : ApiCosmos<WatchedList>(factory, ApiType.Authenticated, "watchedlist", ApiContext.Default.WatchedList)
{
    public async Task<WatchedList?> Get(ComponentActions<WatchedList?>? actions, CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.Get, false, actions, cancellationToken);
    }

    public async Task<WatchedList?> Add(MediaType? mediaType, WatchedList? obj, string? tmdbId, AccountProduct? product, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(tmdbId);
        SubscriptionHelper.ValidateWatched(product, (obj?.Items(mediaType).Count ?? 0) + 1);

        return await PostAsync(Endpoint.Add(mediaType, tmdbId), null, ApiContext.Default.WatchedList, cancellationToken);
    }

    public async Task<WatchedList?> Remove(MediaType? mediaType, string? tmdbId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(tmdbId);

        return await PostAsync(Endpoint.Remove(mediaType, tmdbId), null, ApiContext.Default.WatchedList, cancellationToken);
    }

    private struct Endpoint
    {
        public const string Get = "watchedlist/get";

        public static string Add(MediaType? type, string tmdbId)
        {
            return $"watchedlist/add/{type}/{tmdbId}";
        }

        public static string Remove(MediaType? type, string tmdbId)
        {
            return $"watchedlist/remove/{type}/{tmdbId}";
        }
    }
}