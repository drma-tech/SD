using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Suggestions.Resources;
using SD.WEB.Modules.Suggestions.Interface;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class TmdbDiscoveryApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiServices(factory, memoryCache), IMediaListApi
    {
        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            if (stringParameters != null)
            {
                if (stringParameters.ContainsValue("popularity.desc")) //popularMedia
                {
                    stringParameters.TryAdd("vote_count.gte", "50"); //ignore low-rated movie
                }
                if (stringParameters.ContainsValue("primary_release_date.desc")) //newMedia
                {
                    stringParameters.TryAdd("primary_release_date.lte", DateTime.Now.ToString("yyyy-MM-dd")); //only releasead
                }
                if (stringParameters.ContainsValue("vote_average.desc")) //topRatedMedia
                {
                    stringParameters.TryAdd("primary_release_date.gte", DateTime.Now.AddYears(-20).ToString("yyyy-MM-dd")); //only recent releases
                    stringParameters.TryAdd("vote_count.gte", "500"); //ignore low-rated movie
                    stringParameters.TryAdd("vote_average.gte", "7.4"); //only the best
                }
            }

            var region = stringParameters?.GetValueOrDefault("watch_region");

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
                { "watch_region", region ?? AppStateStatic.Region.ToString() },
                { "include_adult", "false" },
                { "include_video", "false" },
                { "page", page.ToString() }
            };

            if (stringParameters != null)
            {
                foreach (var item in stringParameters)
                {
                    parameter.TryAdd(item.Key, item.Value);
                }
            }

            if (type == null)
            {
                var movies = await GetAsync<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter), true);
                var shows = await GetAsync<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter), true);

                var listOrder = new List<Ordem>();

                listOrder.AddRange(movies?.results.Select(s => new Ordem { id = s.id, type = MediaType.movie, Popularity = s.popularity }) ?? new List<Ordem>());
                listOrder.AddRange(shows?.results.Select(s => new Ordem { id = s.id, type = MediaType.tv, Popularity = s.popularity }) ?? new List<Ordem>());

                foreach (var ordem in listOrder.OrderByDescending(o => o.Popularity))
                {
                    if (ordem.type == MediaType.movie)
                    {
                        if (movies == null) break;
                        var item = movies.results.Single(s => s.id == ordem.id);

                        //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                        currentList.Add(new MediaDetail
                        {
                            tmdb_id = item.id.ToString(),
                            title = item.title,
                            plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
                            release_date = item.release_date?.GetDate(),
                            poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                            poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                            rating = item.vote_count > 5 ? item.vote_average : 0,
                            MediaType = MediaType.movie
                        });
                    }
                    else// if (ordem.type == MediaType.tv)
                    {
                        if (shows == null) break;
                        var item = shows.results.Single(s => s.id == ordem.id);

                        if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                        currentList.Add(new MediaDetail
                        {
                            tmdb_id = item.id.ToString(),
                            title = item.name,
                            plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
                            release_date = item.first_air_date?.GetDate(),
                            poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                            poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                            rating = item.vote_count > 10 ? item.vote_average : 0,
                            MediaType = MediaType.tv
                        });
                    }
                }

                return new(currentList, true);
            }
            else if (type == MediaType.movie)
            {
                var result = await GetAsync<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? [])
                {
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 5 ? item.vote_average : 0,
                        MediaType = MediaType.movie
                    });
                }

                return new(currentList, page >= result?.total_pages);
            }
            else //if (type == MediaType.tv)
            {
                var result = await GetAsync<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? [])
                {
                    if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? Translations.NoPlot : item.overview,
                        release_date = item.first_air_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = MediaType.tv
                    });
                }

                return new(currentList, page >= result?.total_pages);
            }
        }
    }
}