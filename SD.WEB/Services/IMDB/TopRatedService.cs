using SD.Shared.Helper;
using SD.Shared.Modal;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Imdb;
using SD.WEB.Core;
using System.Globalization;

namespace SD.WEB.Services.IMDB
{
    public class TopRatedService : IMediaListService
    {
        private readonly IConfiguration Configuration;

        public TopRatedService(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        public async Task PopulateListMedia(HttpClient http, IStorageService storage, Settings settings,
            HashSet<MediaDetail> list_media, MediaType type, int qtd = 9, Dictionary<string, string>? ExtraParameters = null)
        {
            var options = Configuration.GetSection(ImdbOptions.Section).Get<ImdbOptions>();

            var parameter = new Dictionary<string, string>()
                {
                    { "apiKey", options.ApiKey }
                };

            if (type == MediaType.movie)
            {
                var result = await http.Get<Top250Data>(storage.Local, options.BaseUri + "Top250Movies".ConfigureParameters(parameter)); //bring 250 records

                foreach (var item in result.Items)
                {
                    //if (item.vote_count < 100) continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                    if (new DateTime(int.Parse(item.Year), 1, 1) < DateTime.Now.AddYears(-20)) continue;

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = new DateTime(int.Parse(item.Year), 1, 1),
                        poster_path_small = item.Image.Replace("/original/", "/128x176/"),
                        //poster_path_185 = string.IsNullOrEmpty(item.poster_path) ? null : poster_path_185 + item.poster_path,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.movie
                    });
                }
            }
            else if (type == MediaType.tv)
            {
                var result = await http.Get<Top250Data>(storage.Local, options.BaseUri + "Top250TVs".ConfigureParameters(parameter)); //bring 250 records

                foreach (var item in result.Items)
                {
                    //if (item.vote_count < 100) continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                    if (new DateTime(int.Parse(item.Year), 1, 1) < DateTime.Now.AddYears(-20)) continue;

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = new DateTime(int.Parse(item.Year), 1, 1),
                        poster_path_small = item.Image.Replace("/original/", "/128x176/"),
                        //poster_path_185 = string.IsNullOrEmpty(item.poster_path) ? null : poster_path_185 + item.poster_path,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.tv
                    });
                }
            }
        }
    }
}