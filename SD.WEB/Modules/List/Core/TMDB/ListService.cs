using Blazored.SessionStorage;
using SD.Shared.Model.List.Tmdb;
using System.Text.Json;

namespace SD.WEB.Modules.List.Core.TMDB
{
    public class ListService
    {
        public static async Task PopulateListMedia(HttpClient http, ISyncSessionStorageService? storage, Settings settings,
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

            var result = await http.GetNew<CustomListNew>(storage, TmdbOptions.BaseUriNew + "list/" + ExtraParameters["list_id"].ToString().ConfigureParameters(parameter));

            if (result != null)
            {
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
}