using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Model.List.Tmdb;
using SD.Shared.Models.List.Tmdb;
using System.Text.Json;

namespace SD.WEB.Modules.List.Core
{
    public class TmdbApi : ApiServices
    {
        public TmdbApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetDiscoverList(MediaType? type, Dictionary<string, string> ExtraParameters, HashSet<MediaDetail> currentList, int page = 1)
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
                var movies = await GetAsync<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter), true);
                var shows = await GetAsync<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter), true);

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

                        currentList.Add(new MediaDetail
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

                        currentList.Add(new MediaDetail
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

                return new(currentList, true);
            }
            else if (type == MediaType.movie)
            {
                var result = await GetAsync<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? new List<ResultMovieDiscover>())
                {
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    currentList.Add(new MediaDetail
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

                return new(currentList, page >= result?.total_pages);
            }
            else //if (type == MediaType.tv)
            {
                var result = await GetAsync<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? new List<ResultTvDiscover>())
                {
                    if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    currentList.Add(new MediaDetail
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

                return new(currentList, page >= result?.total_pages);
            }
        }

        public async Task<HashSet<MediaDetail>> GetListList(Dictionary<string, string> ExtraParameters, HashSet<MediaDetail> currentList, int page = 1)
        {
            if (ExtraParameters == null) throw new ArgumentNullException(nameof(ExtraParameters));

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", Settings.Language.GetName(false) ?? "en-US" },
                { "page", page.ToString() },
                { "sort_by", "original_order.asc" }
            };

            foreach (var item in ExtraParameters)
            {
                parameter.Add(item.Key, item.Value);
            }

            var result = await GetByRequest<CustomListNew>(TmdbOptions.BaseUriNew + "list/" + ExtraParameters["list_id"].ToString().ConfigureParameters(parameter));

            if (result != null)
            {
                foreach (var item in result.results)
                {
                    var tv = item.media_type == "tv";

                    result.comments.TryGetProperty($"{(tv ? "tv" : "movie")}:{item.id}", out JsonElement value);

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = tv ? item.name : item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = tv ? item.first_air_date?.GetDate() : item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = tv ? MediaType.tv : MediaType.movie,
                        comments = value.GetString()
                    });
                }
            }

            return currentList;
        }

        public async Task<MediaDetail> GetMediaDetail(string? tmdb_id, MediaType? type)
        {
            if (tmdb_id == null) throw new ArgumentNullException(nameof(tmdb_id));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", Settings.Language.GetName(false) ?? "en-US" },
                { "append_to_response", "videos" }
            };

            var obj_return = new MediaDetail();

            if (type == MediaType.movie)
            {
                var item = await GetAsync<MovieDetail>(TmdbOptions.BaseUri + "movie/" + tmdb_id.ConfigureParameters(parameter), true);

                if (item != null)
                {
                    obj_return = new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_average,
                        runtime = item.runtime,
                        homepage = item.homepage,
                        Videos = item.videos?.results.Select(s => new Video { id = s.id, key = s.key, name = s.name }).ToList() ?? new List<Video>(),
                        Genres = item.genres.Select(s => s.name ?? "").ToList(),
                        MediaType = MediaType.movie
                    };
                }
            }
            else
            {
                var item = await GetAsync<TVDetail>(TmdbOptions.BaseUri + "tv/" + tmdb_id.ConfigureParameters(parameter), true);

                if (item != null)
                {
                    obj_return = new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
                        release_date = item.first_air_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_average,
                        runtime = item.episode_run_time.FirstOrDefault(),
                        homepage = item.homepage,
                        Videos = item.videos?.results.Select(s => new Video { id = s.id, key = s.key, name = s.name }).ToList() ?? new List<Video>(),
                        Genres = item.genres.Select(s => s.name ?? "").ToList(),
                        MediaType = MediaType.tv
                    };
                }
            }

            return obj_return;
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetNowPlayingList(HashSet<MediaDetail> currentList, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", Settings.Language.GetName(false) ?? "en-US" },
                { "region", Settings.Region.ToString() },
                { "page", page.ToString() }
            };

            var result = await GetAsync<MovieNowPlaying>(TmdbOptions.BaseUri + "movie/now_playing".ConfigureParameters(parameter), true);

            foreach (var item in result?.results ?? new List<ResultMovieNowPlaying>())
            {
                //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                if (item.release_date?.GetDate() > DateTime.Today.AddDays(1)) continue; //only accepts titles that will be released no later than one day after today

                currentList.Add(new MediaDetail
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

            return new(currentList, page >= result?.total_pages);
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetTopRatedList(MediaType type, HashSet<MediaDetail> currentList, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "region", Settings.Region.ToString() },
                { "language", Settings.Language.GetName(false) ?? "en-US" },
                { "page", page.ToString() }
            };

            if (type == MediaType.movie)
            {
                var result = await GetAsync<MovieTopRated>(TmdbOptions.BaseUri + "movie/top_rated".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? new List<ResultMovieTopRated>())
                {
                    if (item.release_date?.GetDate() < DateTime.Now.AddYears(-20)) continue;
                    if (item.vote_count < 1000) continue;
                    //if (string.IsNullOrEmpty(item.poster_path)) continue;

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_average,
                        MediaType = MediaType.movie
                    });
                }

                return new(currentList, page >= result?.total_pages);
            }
            else// if (type == MediaType.tv)
            {
                var result = await GetAsync<TVTopRated>(TmdbOptions.BaseUri + "tv/top_rated".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? new List<ResultTVTopRated>())
                {
                    if (item.first_air_date?.GetDate() < DateTime.Now.AddYears(-20)) continue;
                    if (item.vote_count < 1000) continue;
                    if (string.IsNullOrEmpty(item.poster_path)) continue;

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = item.first_air_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_average,
                        MediaType = MediaType.tv
                    });
                }

                return new(currentList, page >= result?.total_pages);
            }
        }

        public async Task<MediaProviders?> GetWatchProvidersList(string? tmdb_id, MediaType? type)
        {
            if (tmdb_id == null) throw new ArgumentNullException(nameof(tmdb_id));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey }
            };

            if (type == MediaType.movie)
            {
                return await GetAsync<MediaProviders>(TmdbOptions.BaseUri + $"movie/{tmdb_id}/watch/providers".ConfigureParameters(parameter), true);
            }
            else //tv
            {
                return await GetAsync<MediaProviders>(TmdbOptions.BaseUri + $"tv/{tmdb_id}/watch/providers".ConfigureParameters(parameter), true);
            }
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetUpcomingList(MediaType type, HashSet<MediaDetail> currentList, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "region", Settings.Region.ToString() },
                { "language", Settings.Language.GetName(false) ?? "en-US" },
                { "page", page.ToString() }
            };

            if (type == MediaType.movie)
            {
                var result = await GetAsync<MovieUpcoming>(TmdbOptions.BaseUri + "movie/upcoming".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? new List<ResultMovieUpcoming>())
                {
                    //if (string.IsNullOrEmpty(item.poster_path)) continue;

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = MediaType.movie
                    });
                }

                return new(currentList, page >= result?.total_pages);
            }
            else// if (type == MediaType.tv)
            {
                throw new NotImplementedException();
            }
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetPopularList(MediaType? type, HashSet<MediaDetail> currentList, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
                {
                    { "api_key", TmdbOptions.ApiKey },
                    //{ "region", settings.Region.ToString() }, //region doesnt affect popular list
                    { "language", Settings.Language.GetName(false) ?? "en-US" },
                    { "page", page.ToString() }
                };

            if (type == null)
            {
                var movies = await GetAsync<MoviePopular>(TmdbOptions.BaseUri + "movie/popular".ConfigureParameters(parameter), true);
                var shows = await GetAsync<TVPopular>(TmdbOptions.BaseUri + "tv/popular".ConfigureParameters(parameter), true);

                var list = new List<Ordem>();

                list.AddRange(movies?.results.Select(s => new Ordem { id = s.id, type = MediaType.movie, Popularity = s.popularity }) ?? new List<Ordem>());
                list.AddRange(shows?.results.Select(s => new Ordem { id = s.id, type = MediaType.tv, Popularity = s.popularity }) ?? new List<Ordem>());

                foreach (var ordem in list.OrderByDescending(o => o.Popularity))
                {
                    if (ordem.type == MediaType.movie)
                    {
                        if (movies == null) break;
                        var item = movies.results.Single(s => s.id == ordem.id);

                        if (item.vote_count < 50) continue; //ignore low-rated movie
                        //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                        currentList.Add(new MediaDetail
                        {
                            tmdb_id = item.id.ToString(),
                            title = item.title,
                            plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                            release_date = item.release_date?.GetDate(),
                            poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                            poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                            rating = item.vote_count > 10 ? item.vote_average : 0,
                            MediaType = MediaType.movie
                        });
                    }
                    else// if (ordem.type == MediaType.tv)
                    {
                        if (shows == null) break;
                        var item = shows.results.Single(s => s.id == ordem.id);

                        if (item.vote_count < 50) continue; //ignore low-rated movie
                        if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                        currentList.Add(new MediaDetail
                        {
                            tmdb_id = item.id.ToString(),
                            title = item.name,
                            plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
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
                var result = await GetAsync<MoviePopular>(TmdbOptions.BaseUri + "movie/popular".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? new List<ResultMoviePopular>())
                {
                    if (item.vote_count < 50) continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = MediaType.movie
                    });
                }

                return new(currentList, page >= result?.total_pages);
            }
            else //if (type == MediaType.tv)
            {
                var result = await GetAsync<TVPopular>(TmdbOptions.BaseUri + "tv/popular".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? new List<ResultTVPopular>())
                {
                    if (item.vote_count < 50) continue; //ignore low-rated movie
                    if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
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

    internal sealed class Ordem
    {
        public MediaType type { get; set; }
        public int id { get; set; }
        public double Popularity { get; set; }
    }
}