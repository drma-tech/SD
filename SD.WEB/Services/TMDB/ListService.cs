using Microsoft.Extensions.Configuration;
using SD.Shared.Helper;
using SD.Shared.Modal;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Tmdb;
using SD.WEB.Core;
using SD.WEB.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SD.Shared.Core;

namespace SD.WEB.Services.TMDB
{
    public class ListService : IMediaListService
    {
        private readonly IConfiguration Configuration;

        public ListService(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        public async Task PopulateListMedia(HttpClient http, IStorageService storage, Settings settings,
            HashSet<MediaDetail> list_media, MediaType type, int qtd = 9, Dictionary<string, string> ExtraParameters = null)
        {
            if (ExtraParameters == null) throw new ArgumentNullException(nameof(ExtraParameters));

            if (qtd > 9)
            {
                return;
            }

            var options = Configuration.GetSection(TmdbOptions.Section).Get<TmdbOptions>();

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", options.ApiKey },
                { "language", settings.Language.GetName(false) },
                { "page", "1" },
                { "sort_by", "original_order.asc" }
            };

            if (ExtraParameters != null)
            {
                foreach (var item in ExtraParameters)
                {
                    parameter.Add(item.Key, item.Value);
                }
            }

            var result = await http.GetNew<CustomListNew>(storage.Local, options.BaseUriNew + "list/" + ExtraParameters["list_id"].ToString().ConfigureParameters(parameter));

            foreach (var item in result.results)
            {
                var tv = item.media_type == "tv";

                result.comments.TryGetProperty($"{(tv ? "tv" : "movie")}:{item.id}", out JsonElement value);

                list_media.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = tv ? item.name : item.title,
                    plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    release_date = tv ? item.first_air_date.GetDate() : item.release_date.GetDate(),
                    poster_path_small = string.IsNullOrEmpty(item.poster_path) ? null : options.SmallPosterPath + item.poster_path,
                    poster_path_large = string.IsNullOrEmpty(item.poster_path) ? null : options.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 10 ? item.vote_average : 0,
                    MediaType = tv ? MediaType.tv : MediaType.movie,
                    comments = value.GetString()
                });
            }
        }
    }
}