using SD.Shared.Core;
using SD.Shared.Helper;
using SD.Shared.Modal;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Tmdb;
using SD.WEB.Core;

namespace SD.WEB.Services.TMDB
{
    public class MediaDetailService
    {
        private readonly IConfiguration Configuration;

        public MediaDetailService(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        public async Task<MediaDetail> GetMedia(HttpClient http, IStorageService storage, Settings settings, string tmdb_id, MediaType type)
        {
            var options = Configuration.GetSection(TmdbOptions.Section).Get<TmdbOptions>();

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", options.ApiKey },
                { "language", settings.Language.GetName(false) },
                { "append_to_response", "videos" }
            };

            MediaDetail obj_return;

            if (type == MediaType.movie)
            {
                var item = await http.Get<MovieDetail>(options.BaseUri + "movie/" + tmdb_id.ConfigureParameters(parameter));

                obj_return = new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.title,
                    plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
                    release_date = item.release_date.GetDate(),
                    poster_path_small = string.IsNullOrEmpty(item.poster_path) ? null : options.SmallPosterPath + item.poster_path,
                    poster_path_large = string.IsNullOrEmpty(item.poster_path) ? null : options.LargePosterPath + item.poster_path,
                    rating = item.vote_average,
                    runtime = item.runtime,
                    homepage = item.homepage,
                    Videos = item.videos.results.Select(s => new Video { id = s.id, key = s.key, name = s.name }).ToList(),
                    Genres = item.genres.Select(s => s.name).ToList(),
                    MediaType = MediaType.movie
                };
            }
            else
            {
                var item = await http.Get<TVDetail>(options.BaseUri + "tv/" + tmdb_id.ConfigureParameters(parameter));

                obj_return = new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.name,
                    plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
                    release_date = item.first_air_date.GetDate(),
                    poster_path_small = string.IsNullOrEmpty(item.poster_path) ? null : options.SmallPosterPath + item.poster_path,
                    poster_path_large = string.IsNullOrEmpty(item.poster_path) ? null : options.LargePosterPath + item.poster_path,
                    rating = item.vote_average,
                    runtime = item.episode_run_time.FirstOrDefault(),
                    homepage = item.homepage,
                    Videos = item.videos.results.Select(s => new Video { id = s.id, key = s.key, name = s.name }).ToList(),
                    Genres = item.genres.Select(s => s.name).ToList(),
                    MediaType = MediaType.tv
                };
            }

            return obj_return;
        }
    }
}