using Blazored.SessionStorage;
using SD.Shared.Model.List.Imdb;
using System.Globalization;

namespace SD.WEB.Modules.List.Core.IMDB
{
    public static class UpcomingService
    {
        public static async Task PopulateIMDBUpcoming(this HttpClient http, ISyncSessionStorageService storage, HashSet<MediaDetail> list_media, MediaType type)
        {
            var parameter = new Dictionary<string, string>()
                {
                    { "apiKey", ImdbOptions.ApiKey }
                };

            if (type == MediaType.movie)
            {
                var result = await http.Get<NewMovieData>(ImdbOptions.BaseUri + "ComingSoon".ConfigureParameters(parameter), true, storage); //undefined numeric record

                foreach (var item in result?.Items ?? new List<NewMovieDataDetail>())
                {
                    //if (item.vote_count < 100) continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = DateTime.Parse(item.ReleaseState ?? DateTime.MinValue.ToString(), CultureInfo.InvariantCulture),
                        //poster_path_small = ImdbOptions.ResizeImage + item.Image,
                        poster_small = item.Image,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.movie
                    });
                }
            }
            else if (type == MediaType.tv)
            {
                throw new NotImplementedException();
            }
        }
    }
}