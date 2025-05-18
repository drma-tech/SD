﻿using SD.Shared.Models.Auth;
using SD.WEB.Shared;

namespace SD.WEB.Modules.Profile.Core;

public class WatchedListApi(IHttpClientFactory factory) : ApiCosmos<WatchedList>(factory, "watchedlist")
{
    public async Task<WatchedList?> Get(bool IsUserAuthenticated, RenderControlCore<WatchedList?>? core,
        string? id = null)
    {
        if (!string.IsNullOrEmpty(id)) return await GetAsync($"{Endpoint.Get}?id={id}", core);

        if (IsUserAuthenticated) return await GetAsync(Endpoint.Get, core);

        return new WatchedList();
    }

    public async Task<WatchedList?> Add(MediaType? mediaType, WatchedList? obj, string? TmdbId, ClientePaddle? paddle)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(TmdbId);
        SubscriptionHelper.ValidateWatched(paddle?.ActiveProduct, (obj?.Items(mediaType).Count ?? 0) + 1);

        return await PostAsync<WatchedList>(Endpoint.Add(mediaType, TmdbId), null, null);
    }

    public async Task<WatchedList?> Remove(MediaType? mediaType, string? TmdbId)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(TmdbId);

        return await PostAsync<WatchedList>(Endpoint.Remove(mediaType, TmdbId), null, null);
    }

    private struct Endpoint
    {
        public const string Get = "public/watchedlist/get";

        public static string Add(MediaType? type, string TmdbId)
        {
            return $"watchedlist/add/{type}/{TmdbId}";
        }

        public static string Remove(MediaType? type, string TmdbId)
        {
            return $"watchedlist/remove/{type}/{TmdbId}";
        }
    }
}