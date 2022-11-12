using Blazored.SessionStorage;
using SD.Shared.Model.List.Tmdb;

namespace SD.WEB.Modules.List.Core.TMDB
{
    public static class DiscoverService
    {
        public static async Task<bool> PopulateDiscover(this HttpClient http, ISyncSessionStorageService? storage,
            HashSet<MediaDetail> list_media, MediaType? type, int page = 1, Dictionary<string, string>? ExtraParameters = null)
        {
            if (ExtraParameters != null)
            {
                if (ExtraParameters.ContainsValue("popularity.desc")) //popularMedia
                {
                    ExtraParameters.TryAdd("vote_count.gte", "50"); //ignore low-rated movie
                }
                if (ExtraParameters.ContainsValue("primary_release_date.desc")) //newMedia
                {
                    ExtraParameters.TryAdd("primary_release_date.lte", DateTime.Now.ToString("yyyy-MM-dd")); //only releasead
                }
                if (ExtraParameters.ContainsValue("vote_average.desc")) //topRatedMedia
                {
                    ExtraParameters.TryAdd("primary_release_date.gte", DateTime.Now.AddYears(-20).ToString("yyyy-MM-dd")); //only recent releases
                    ExtraParameters.TryAdd("vote_count.gte", "1000"); //ignore low-rated movie
                    ExtraParameters.TryAdd("vote_average.gte", "7.4"); //only the best
                }
            }

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", Settings.Language.GetName(false) ?? "en-US" },
                { "watch_region", Settings.Region.ToString() },
                { "page", page.ToString() }
            };

            if (ExtraParameters != null)
            {
                foreach (var item in ExtraParameters)
                {
                    parameter.TryAdd(item.Key, item.Value);
                }
            }

            if (type == null)
            {
                var movies = await http.Get<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter), true, storage);
                var shows = await http.Get<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter), true, storage);

                var list = new List<Ordem>();

                list.AddRange(movies?.results.Select(s => new Ordem { id = s.id, type = MediaType.movie, Popularity = s.popularity }) ?? new List<Ordem>());
                list.AddRange(shows?.results.Select(s => new Ordem { id = s.id, type = MediaType.tv, Popularity = s.popularity }) ?? new List<Ordem>());

                foreach (var ordem in list.OrderByDescending(o => o.Popularity))
                {
                    if (ordem.type == MediaType.movie)
                    {
                        if (movies == null) break;
                        var item = movies.results.Single(s => s.id == ordem.id);

                        //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                        list_media.Add(new MediaDetail
                        {
                            tmdb_id = item.id.ToString(),
                            title = item.title,
                            plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
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

                        list_media.Add(new MediaDetail
                        {
                            tmdb_id = item.id.ToString(),
                            title = item.name,
                            plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
                            release_date = item.first_air_date?.GetDate(),
                            poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                            poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                            rating = item.vote_count > 10 ? item.vote_average : 0,
                            MediaType = MediaType.tv
                        });
                    }
                }

                return true;
            }
            else if (type == MediaType.movie)
            {
                var result = await http.Get<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter), true, storage);

                foreach (var item in result?.results ?? new List<ResultMovieDiscover>())
                {
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 5 ? item.vote_average : 0,
                        MediaType = MediaType.movie
                    });
                }

                return page >= result?.total_pages;
            }
            else //if (type == MediaType.tv)
            {
                var result = await http.Get<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter), true, storage);

                foreach (var item in result?.results ?? new List<ResultTvDiscover>())
                {
                    if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
                        release_date = item.first_air_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = MediaType.tv
                    });
                }

                return page >= result?.total_pages;
            }
        }
    }
}