using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Suggestions.Interface;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class TmdbPopularApi : ApiServices, IMediaListApi
    {
        public TmdbPopularApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
                {
                    { "api_key", TmdbOptions.ApiKey },
                    //{ "region", AppStateStatic.Region.ToString() }, //region doesnt affect popular list
                    { "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
                    { "page", page.ToString() }
                };

            if (type == null)
            {
                var movies = await GetAsync<MoviePopular>(TmdbOptions.BaseUri + "movie/popular".ConfigureParameters(parameter), true);
                var shows = await GetAsync<TVPopular>(TmdbOptions.BaseUri + "tv/popular".ConfigureParameters(parameter), true);

                var listOrder = new List<Ordem>();

                listOrder.AddRange(movies?.results.Select(s => new Ordem { id = s.id, type = MediaType.movie, Popularity = s.popularity }) ?? new List<Ordem>());
                listOrder.AddRange(shows?.results.Select(s => new Ordem { id = s.id, type = MediaType.tv, Popularity = s.popularity }) ?? new List<Ordem>());

                foreach (var ordem in listOrder.OrderByDescending(o => o.Popularity))
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
}