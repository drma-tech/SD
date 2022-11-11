using Blazored.SessionStorage;
using SD.Shared.Model.List.Tmdb;

namespace SD.WEB.Modules.List.Core.TMDB
{
    internal sealed class Ordem
    {
        public MediaType type { get; set; }
        public int id { get; set; }
        public double Popularity { get; set; }
    }

    public static class PopularService
    {
        public static async Task<bool> PopulateTMDBPopular(this HttpClient http, ISyncSessionStorageService? storage, Settings settings,
            HashSet<MediaDetail> list_media, MediaType? type = null, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
                {
                    { "api_key", TmdbOptions.ApiKey },
                    //{ "region", settings.Region.ToString() }, //region doesnt affect popular list
                    { "language", settings.Language.GetName(false) ?? "en-US" },
                    { "page", page.ToString() }
                };

            if (type == null)
            {
                var movies = await http.Get<MoviePopular>(TmdbOptions.BaseUri + "movie/popular".ConfigureParameters(parameter), true, storage);
                var shows = await http.Get<TVPopular>(TmdbOptions.BaseUri + "tv/popular".ConfigureParameters(parameter), true, storage);

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

                        list_media.Add(new MediaDetail
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

                        list_media.Add(new MediaDetail
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

                return true;
            }
            else if (type == MediaType.movie)
            {
                var result = await http.Get<MoviePopular>(TmdbOptions.BaseUri + "movie/popular".ConfigureParameters(parameter), true, storage);

                foreach (var item in result?.results ?? new List<ResultMoviePopular>())
                {
                    if (item.vote_count < 50) continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    list_media.Add(new MediaDetail
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

                return page >= result?.total_pages;
            }
            else //if (type == MediaType.tv)
            {
                var result = await http.Get<TVPopular>(TmdbOptions.BaseUri + "tv/popular".ConfigureParameters(parameter), true, storage);

                foreach (var item in result?.results ?? new List<ResultTVPopular>())
                {
                    if (item.vote_count < 50) continue; //ignore low-rated movie
                    if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    list_media.Add(new MediaDetail
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

                return page >= result?.total_pages;
            }
        }
    }
}