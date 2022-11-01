using Blazored.SessionStorage;
using SD.Shared.Model.List.Imdb;
using System.Globalization;

namespace SD.WEB.Modules.List.Core.IMDB
{
    public static class TopRatedService
    {
        public static async Task PopulateIMDBTopRated(this HttpClient http, ISyncSessionStorageService storage, HashSet<MediaDetail> list_media, MediaType type)
        {
            var parameter = new Dictionary<string, string>()
                {
                    { "apiKey", ImdbOptions.ApiKey }
                };

            if (type == MediaType.movie)
            {
                var result = await http.Get<Top250Data>(ImdbOptions.BaseUri + "Top250Movies".ConfigureParameters(parameter), true, storage); //bring 250 records

                foreach (var item in result?.Items ?? new List<Top250DataDetail>())
                {
                    //if (item.vote_count < 100) continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                    if (new DateTime(int.Parse(item.Year ?? "0"), 1, 1) < DateTime.Now.AddYears(-20)) continue;

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = new DateTime(int.Parse(item.Year ?? "0"), 1, 1),
                        poster_small = ImdbOptions.ResizeImage + item.Image,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.movie
                    });
                }
            }
            else if (type == MediaType.tv)
            {
                var result = await http.Get<Top250Data>(ImdbOptions.BaseUri + "Top250TVs".ConfigureParameters(parameter), true, storage); //bring 250 records

                foreach (var item in result?.Items ?? new List<Top250DataDetail>())
                {
                    //if (item.vote_count < 100) continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                    if (new DateTime(int.Parse(item.Year ?? "0"), 1, 1) < DateTime.Now.AddYears(-20)) continue;

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = new DateTime(int.Parse(item.Year ?? "0"), 1, 1),
                        poster_small = ImdbOptions.ResizeImage + item.Image,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.tv
                    });
                }
            }
        }
    }
}