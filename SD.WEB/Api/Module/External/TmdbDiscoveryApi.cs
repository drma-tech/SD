using SD.Shared.Models.List.Tmdb;
using SD.WEB.Api.Core;
using System.Globalization;

namespace SD.WEB.Api.Module.External;

public class TmdbDiscoveryApi(IHttpClientFactory factory) : ApiExternal(factory), IMediaListApi
{
    public async Task<(ISet<MediaDetail> list, bool lastPage)> GetList(ISet<MediaDetail> currentList, RenderControlState<ISet<MediaDetail>>[] states,
        MediaType? type = null, IDictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1, CancellationToken cancellationToken = default)
    {
        if (currentList.Empty())
        {
            foreach (var state in states)
            {
                await state.StartLoading(null);
            }
        }

        if (stringParameters != null)
        {
            if (type == MediaType.tv && string.Equals(stringParameters["sort_by"], "primary_release_date.desc", StringComparison.OrdinalIgnoreCase))
            {
                stringParameters["sort_by"] = "first_air_date.desc";
            }

            if (stringParameters.Values.Contains("popularity.desc")) //popularMedia
                stringParameters.TryAdd("vote_count.gte", "25"); //ignore low-rated movie
            if (stringParameters.Values.Contains("primary_release_date.desc")) //newMedia
                stringParameters.TryAdd("primary_release_date.lte", DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)); //only released
            if (stringParameters.Values.Contains("vote_average.desc")) //topRatedMedia
            {
                stringParameters.TryAdd("primary_release_date.gte", DateTime.Now.AddYears(-30).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)); //only recent releases
                stringParameters.TryAdd("vote_count.gte", "250"); //ignore low-rated movie
                stringParameters.TryAdd("vote_average.gte", "7"); //only the best
            }
        }

        var region = stringParameters != null && stringParameters.TryGetValue("watch_region", out var value) ? value : null;

        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", (await AppStateStatic.GetContentLanguage(cancellationToken: cancellationToken)).GetFieldSettings(translate: false).Name ?? "en-US" },
            { "watch_region", region?.ToUpperInvariant() ?? (await AppStateStatic.GetRegion(api: null, js: null, cancellationToken)).ToString().ToUpperInvariant() },
            { "include_adult", "false" },
            { "include_video", "false" },
            { "page", page.ToString(CultureInfo.InvariantCulture) },
        };

        if (stringParameters != null)
            foreach (var item in stringParameters)
                parameter.TryAdd(item.Key, item.Value);

        if (string.Equals(parameter["watch_region"], "NONE", StringComparison.OrdinalIgnoreCase))
            parameter.Remove("watch_region");

        if (type == null)
        {
            var movies = await GetAsync<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter), setNewVersion: false, states: [], cancellationToken);
            var shows = await GetAsync<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter), setNewVersion: false, states: [], cancellationToken);

            var listOrder = new List<Order>();

            listOrder.AddRange(movies?.results.Select(s => new Order { Id = s.id, Type = MediaType.movie, Popularity = s.popularity }) ?? []);
            listOrder.AddRange(shows?.results.Select(s => new Order { Id = s.id, Type = MediaType.tv, Popularity = s.popularity }) ?? []);

            foreach (var ordem in listOrder.OrderByDescending(o => o.Popularity))
                if (ordem.Type == MediaType.movie)
                {
                    if (movies == null) break;
                    var item = movies.results.Single(s => s.id == ordem.Id);

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? Translations.Module.Media.NoPlot : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path)
                            ? null
                            : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path)
                            ? null
                            : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 5 ? item.vote_average : 0,
                        MediaType = MediaType.movie,
                    });
                }
                else // if (ordem.type == MediaType.tv)
                {
                    if (shows == null) break;
                    var item = shows.results.Single(s => s.id == ordem.Id);

                    if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
                        title = item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? Translations.Module.Media.NoPlot : item.overview,
                        release_date = item.first_air_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path)
                            ? null
                            : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path)
                            ? null
                            : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = MediaType.tv,
                    });
                }

            foreach (var state in states)
            {
                await state.FinishLoading(currentList);
            }
            return new ValueTuple<ISet<MediaDetail>, bool>(currentList, page >= movies?.total_pages && page >= shows?.total_pages);
        }

        if (type == MediaType.movie)
        {
            var result = await GetAsync<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter), setNewVersion: false, states: [], cancellationToken);

            foreach (var item in result?.results ?? [])
                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
                    title = item.title,
                    plot = string.IsNullOrEmpty(item.overview) ? Translations.Module.Media.NoPlot : item.overview,
                    release_date = item.release_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 5 ? item.vote_average : 0,
                    MediaType = MediaType.movie,
                });

            foreach (var state in states)
            {
                await state.FinishLoading(currentList);
            }
            return new ValueTuple<ISet<MediaDetail>, bool>(currentList, page >= result?.total_pages);
        }
        else //if (type == MediaType.tv)
        {
            var result = await GetAsync<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter), setNewVersion: false, states: [], cancellationToken);

            foreach (var item in result?.results ?? [])
            {
                if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
                    title = item.name,
                    plot = string.IsNullOrEmpty(item.overview) ? Translations.Module.Media.NoPlot : item.overview,
                    release_date = item.first_air_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 10 ? item.vote_average : 0,
                    MediaType = MediaType.tv,
                });
            }

            foreach (var state in states)
            {
                await state.FinishLoading(currentList);
            }
            return new ValueTuple<ISet<MediaDetail>, bool>(currentList, page >= result?.total_pages);
        }
    }
}