﻿using SD.Shared.Models.Auth;
using SD.WEB.Shared;

namespace SD.WEB.Modules.Profile.Core;

public class WatchingListApi(IHttpClientFactory factory) : ApiCosmos<WatchingList>(factory, "watchinglist")
{
    public async Task<WatchingList?> Get(bool IsUserAuthenticated, RenderControlCore<WatchingList?>? core,
        string? id = null)
    {
        if (!string.IsNullOrEmpty(id)) return await GetAsync($"{Endpoint.Get}?id={id}", core);

        if (IsUserAuthenticated) return await GetAsync(Endpoint.Get, core);

        return new WatchingList();
    }

    public async Task<WatchingList?> Add(MediaType? mediaType, WatchingList? obj, WatchingListItem? item,
        ClientePaddle? paddle)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(item);
        SubscriptionHelper.ValidateWatching(paddle?.ActiveProduct, (obj?.Items(mediaType).Count ?? 0) + 1);

        return await PostAsync(Endpoint.Add(mediaType), null, item);
    }

    public async Task<WatchingList?> Remove(MediaType? mediaType, string? CollectionId, string? TmdbId = "null")
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(CollectionId);

        return await PostAsync<WatchingList>(Endpoint.Remove(mediaType, CollectionId, TmdbId ?? "null"), null, null);
    }

    public async Task<WatchingList?> Sync(MediaType? mediaType, WatchingList? obj,
        RenderControlCore<WatchingList?>? core)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync<WatchingList>(Endpoint.Sync(mediaType), core, obj);
    }

    private struct Endpoint
    {
        public const string Get = "public/watchinglist/get";

        public static string Add(MediaType? type)
        {
            return $"watchinglist/add/{type}";
        }

        public static string Remove(MediaType? type, string CollectionId, string TmdbId)
        {
            return $"watchinglist/remove/{type}/{CollectionId}/{TmdbId}";
        }

        public static string Sync(MediaType? type)
        {
            return $"watchinglist/sync/{type}";
        }
    }
}