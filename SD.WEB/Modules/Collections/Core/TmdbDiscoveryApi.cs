﻿using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Collections.Interface;
using SD.WEB.Modules.Collections.Resources;

namespace SD.WEB.Modules.Collections.Core;

public class TmdbDiscoveryApi(IHttpClientFactory factory) : ApiExternal(factory), IMediaListApi
{
    public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList,
        MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null,
        int page = 1)
    {
        if (stringParameters != null)
        {
            if (stringParameters.ContainsValue("popularity.desc")) //popularMedia
                stringParameters.TryAdd("vote_count.gte", "50"); //ignore low-rated movie
            if (stringParameters.ContainsValue("primary_release_date.desc")) //newMedia
                stringParameters.TryAdd("primary_release_date.lte",
                    DateTime.Now.ToString("yyyy-MM-dd")); //only released
            if (stringParameters.ContainsValue("vote_average.desc")) //topRatedMedia
            {
                stringParameters.TryAdd("primary_release_date.gte",
                    DateTime.Now.AddYears(-20).ToString("yyyy-MM-dd")); //only recent releases
                stringParameters.TryAdd("vote_count.gte", "500"); //ignore low-rated movie
                stringParameters.TryAdd("vote_average.gte", "7.4"); //only the best
            }
        }

        var region = stringParameters?.GetValueOrDefault("watch_region");

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", AppStateStatic.ContentLanguage.GetName(false) ?? "en-US" },
            { "watch_region", region ?? AppStateStatic.Region.ToString() },
            { "include_adult", "false" },
            { "include_video", "false" },
            { "page", page.ToString() }
        };

        if (stringParameters != null)
            foreach (var item in stringParameters)
                parameter.TryAdd(item.Key, item.Value);

        if (type == null)
        {
            var movies =
                await GetAsync<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter));
            var shows = await GetAsync<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter));

            var listOrder = new List<Order>();

            listOrder.AddRange(movies?.results.Select(s => new Order
                { Id = s.id, Type = MediaType.movie, Popularity = s.popularity }) ?? []);
            listOrder.AddRange(shows?.results.Select(s => new Order
                { Id = s.id, Type = MediaType.tv, Popularity = s.popularity }) ?? []);

            foreach (var ordem in listOrder.OrderByDescending(o => o.Popularity))
                if (ordem.Type == MediaType.movie)
                {
                    if (movies == null) break;
                    var item = movies.results.Single(s => s.id == ordem.Id);

                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path)
                            ? null
                            : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path)
                            ? null
                            : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 5 ? item.vote_average : 0,
                        MediaType = MediaType.movie
                    });
                }
                else // if (ordem.type == MediaType.tv)
                {
                    if (shows == null) break;
                    var item = shows.results.Single(s => s.id == ordem.Id);

                    if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
                        release_date = item.first_air_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path)
                            ? null
                            : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path)
                            ? null
                            : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = MediaType.tv
                    });
                }

            return new ValueTuple<HashSet<MediaDetail>, bool>(currentList, true);
        }

        if (type == MediaType.movie)
        {
            var result =
                await GetAsync<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter));

            foreach (var item in result?.results ?? [])
                //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.title,
                    plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
                    release_date = item.release_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 5 ? item.vote_average : 0,
                    MediaType = MediaType.movie
                });

            return new ValueTuple<HashSet<MediaDetail>, bool>(currentList, page >= result?.total_pages);
        }
        else //if (type == MediaType.tv)
        {
            var result = await GetAsync<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter));

            foreach (var item in result?.results ?? [])
            {
                if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.name,
                    plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
                    release_date = item.first_air_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 10 ? item.vote_average : 0,
                    MediaType = MediaType.tv
                });
            }

            return new ValueTuple<HashSet<MediaDetail>, bool>(currentList, page >= result?.total_pages);
        }
    }
}