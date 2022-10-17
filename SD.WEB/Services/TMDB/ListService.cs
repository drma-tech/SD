using SD.Shared.Core;
using SD.Shared.Helper;
using SD.Shared.Modal;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Tmdb;
using SD.WEB.Core;
using System.Text.Json;

namespace SD.WEB.Services.TMDB
{
    public class ListService
    {
        public async Task PopulateListMedia(HttpClient http, IStorageService storage, Settings settings,
            HashSet<MediaDetail> list_media, int page = 1, Dictionary<string, string>? ExtraParameters = null)
        {
            if (ExtraParameters == null) throw new ArgumentNullException(nameof(ExtraParameters));

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", settings.Language.GetName(false) ?? "en-US" },
                { "page", page.ToString() },
                { "sort_by", "original_order.asc" }
            };

            foreach (var item in ExtraParameters)
            {
                parameter.Add(item.Key, item.Value);
            }

            var result = await http.GetNew<CustomListNew>(storage.Local, TmdbOptions.BaseUriNew + "list/" + ExtraParameters["list_id"].ToString().ConfigureParameters(parameter));

            foreach (var item in result.results)
            {
                var tv = item.media_type == "tv";

                result.comments.TryGetProperty($"{(tv ? "tv" : "movie")}:{item.id}", out JsonElement value);

                list_media.Add(new MediaDetail
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
    }
}