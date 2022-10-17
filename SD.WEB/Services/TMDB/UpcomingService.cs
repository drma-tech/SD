using SD.Shared.Core;
using SD.Shared.Helper;
using SD.Shared.Modal;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Tmdb;
using SD.WEB.Core;

namespace SD.WEB.Services.TMDB
{
    public static class UpcomingService
    {
        public static async Task<bool> PopulateTMDBUpcoming(this HttpClient http, IStorageService storage, Settings settings,
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
                var result = await http.Get<MovieUpcoming>(TmdbOptions.BaseUri + "movie/upcoming".ConfigureParameters(parameter), true, storage.Session);

                foreach (var item in result?.results ?? new List<ResultMovieUpcoming>())
                {
                    //if (string.IsNullOrEmpty(item.poster_path)) continue;

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
            else// if (type == MediaType.tv)
            {
                throw new NotImplementedException();
            }
        }
    }
}