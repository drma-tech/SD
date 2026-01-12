using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Profile.Core;

public class WatchingListApi(IHttpClientFactory factory) : ApiCosmos<WatchingList>(factory, ApiType.Authenticated, "watchinglist")
{
    public async Task<WatchingList?> Get(bool isUserAuthenticated)
    {
        if (isUserAuthenticated) return await GetAsync(Endpoint.Get);

        return new WatchingList();
    }

    public async Task<WatchingList?> Add(MediaType? mediaType, WatchingList? obj, WatchingListItem? item, AuthSubscription? subs)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(item);
        SubscriptionHelper.ValidateWatching(subs?.ActiveProduct, (obj?.Items(mediaType).Count ?? 0) + 1);

        return await PostAsync(Endpoint.Add(mediaType), item);
    }

    public async Task<WatchingList?> Remove(MediaType? mediaType, string? collectionId, string? tmdbId = "null")
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(collectionId);

        return await PostAsync<WatchingList>(Endpoint.Remove(mediaType, collectionId, tmdbId ?? "null"), null);
    }

    public async Task<WatchingList?> Sync(MediaType? mediaType, WatchingList? obj)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync<WatchingList>(Endpoint.Sync(mediaType), obj);
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