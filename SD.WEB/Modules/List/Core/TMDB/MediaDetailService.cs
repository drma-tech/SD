using SD.Shared.Model.List.Tmdb;

namespace SD.WEB.Modules.List.Core.TMDB
{
    public class MediaDetailService
    {
        public static async Task<MediaDetail> GetMedia(HttpClient http, string? tmdb_id, MediaType? type)
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
                var item = await http.Get<MovieDetail>(TmdbOptions.BaseUri + "movie/" + tmdb_id.ConfigureParameters(parameter), true);

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
                var item = await http.Get<TVDetail>(TmdbOptions.BaseUri + "tv/" + tmdb_id.ConfigureParameters(parameter), true);

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
    }
}