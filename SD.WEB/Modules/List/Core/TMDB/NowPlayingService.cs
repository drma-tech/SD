using Blazored.SessionStorage;
using SD.Shared.Models.List.Tmdb;

namespace SD.WEB.Modules.List.Core.TMDB
{
    public static class NowPlayingService
    {
        public static async Task<bool> PopulateNowPlaying(this HttpClient http, ISyncSessionStorageService storage, Settings settings,
            HashSet<MediaDetail> list_media, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", settings.Language.GetName(false) ?? "en-US" },
                { "region", settings.Region.ToString() },
                { "page", page.ToString() }
            };

            var result = await http.Get<MovieNowPlaying>(TmdbOptions.BaseUri + "movie/now_playing".ConfigureParameters(parameter), true, storage);

            foreach (var item in result?.results ?? new List<ResultMovieNowPlaying>())
            {
                //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                if (item.release_date?.GetDate() > DateTime.Today.AddDays(1)) continue; //only accepts titles that will be released no later than one day after today

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
    }
}