﻿using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Collections.Interface;

namespace SD.WEB.Modules.Collections.Core;

public class TmdbPopularApi(IHttpClientFactory factory) : ApiExternal(factory), IMediaListApi
{
    public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList,
        MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null,
        int page = 1)
    {
        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            //{ "region", AppStateStatic.Region.ToString() }, //region doesn't affect popular list
            { "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
            { "page", page.ToString() }
        };

        if (type == null)
        {
            var movies =
                await GetAsync<MoviePopular>(TmdbOptions.BaseUri + "movie/popular".ConfigureParameters(parameter));
            var shows = await GetAsync<TVPopular>(TmdbOptions.BaseUri + "tv/popular".ConfigureParameters(parameter));

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

                    if (item.vote_count < 50) continue; //ignore low-rated movie

                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path)
                            ? null
                            : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path)
                            ? null
                            : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = MediaType.movie
                    });
                }
                else // if (ordem.type == MediaType.tv)
                {
                    if (shows == null) break;
                    var item = shows.results.Single(s => s.id == ordem.Id);

                    if (item.vote_count < 50) continue; //ignore low-rated movie
                    if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
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
                await GetAsync<MoviePopular>(TmdbOptions.BaseUri + "movie/popular".ConfigureParameters(parameter));

            foreach (var item in result?.results ?? [])
            {
                if (item.vote_count < 50) continue; //ignore low-rated movie
                //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.title,
                    plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    release_date = item.release_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 10 ? item.vote_average : 0,
                    MediaType = MediaType.movie
                });
            }

            return new ValueTuple<HashSet<MediaDetail>, bool>(currentList, page >= result?.total_pages);
        }
        else //if (type == MediaType.tv)
        {
            var result = await GetAsync<TVPopular>(TmdbOptions.BaseUri + "tv/popular".ConfigureParameters(parameter));

            foreach (var item in result?.results ?? [])
            {
                if (item.vote_count < 50) continue; //ignore low-rated movie
                if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.name,
                    plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
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