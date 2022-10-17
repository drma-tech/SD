using SD.Shared.Core;
using SD.Shared.Helper;
using SD.Shared.Modal;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Tmdb;
using SD.WEB.Core;

namespace SD.WEB.Services.TMDB
{
    public static class TopRatedService
    {
        public static async Task<bool> PopulateTMDBTopRated(this HttpClient http, IStorageService storage, Settings settings,
            HashSet<MediaDetail> list_media, MediaType type, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "region", settings.Region.ToString() },
                { "language", settings.Language.GetName(false) ?? "en-US" },
                { "page", page.ToString() }
            };

            if (type == MediaType.movie)
            {
                var result = await http.Get<MovieTopRated>(TmdbOptions.BaseUri + "movie/top_rated".ConfigureParameters(parameter), true, storage.Session);

                foreach (var item in result?.results ?? new List<ResultMovieTopRated>())
                {
                    if (item.release_date?.GetDate() < DateTime.Now.AddYears(-20)) continue;
                    if (item.vote_count < 1000) continue;
                    //if (string.IsNullOrEmpty(item.poster_path)) continue;

                    list_media.Add(new MediaDetail
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

                return page >= result?.total_pages;
            }
            else// if (type == MediaType.tv)
            {
                var result = await http.Get<TVTopRated>(TmdbOptions.BaseUri + "tv/top_rated".ConfigureParameters(parameter), true, storage.Session);

                foreach (var item in result?.results ?? new List<ResultTVTopRated>())
                {
                    if (item.first_air_date?.GetDate() < DateTime.Now.AddYears(-20)) continue;
                    if (item.vote_count < 1000) continue;
                    if (string.IsNullOrEmpty(item.poster_path)) continue;

                    list_media.Add(new MediaDetail
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

                return page >= result?.total_pages;
            }
        }
    }
}