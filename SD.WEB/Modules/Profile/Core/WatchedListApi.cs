using SD.Shared.Models.Auth;
using SD.WEB.Shared;

namespace SD.WEB.Modules.Profile.Core;

public class WatchedListApi(IHttpClientFactory factory) : ApiCosmos<WatchedList>(factory, "watchedlist")
{
    public async Task<WatchedList?> Get(bool isUserAuthenticated, RenderControlCore<WatchedList?>? core,
        string? id = null)
    {
        if (!string.IsNullOrEmpty(id)) return await GetAsync($"{Endpoint.Get}?id={id}", core);

        if (isUserAuthenticated) return await GetAsync(Endpoint.Get, core);

        return new WatchedList();
    }

    public async Task<WatchedList?> Add(MediaType? mediaType, WatchedList? obj, string? tmdbId, ClientePaddle? paddle)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(tmdbId);
        SubscriptionHelper.ValidateWatched(paddle?.ActiveProduct, (obj?.Items(mediaType).Count ?? 0) + 1);

        return await PostAsync<WatchedList>(Endpoint.Add(mediaType, tmdbId), null, null);
    }

    public async Task<WatchedList?> Remove(MediaType? mediaType, string? tmdbId)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(tmdbId);

        return await PostAsync<WatchedList>(Endpoint.Remove(mediaType, tmdbId), null, null);
    }

    private struct Endpoint
    {
        public const string Get = "public/watchedlist/get";

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