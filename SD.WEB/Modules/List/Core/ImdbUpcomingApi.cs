using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Model.List.Imdb;
using SD.WEB.Modules.List.Interface;
using System.Globalization;

namespace SD.WEB.Modules.List.Core
{
    public class ImdbUpcomingApi : ApiServices, IMediaListApi
    {
        public ImdbUpcomingApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
                {
                    { "apiKey", ImdbOptions.ApiKey }
                };

            var list_media = new HashSet<MediaDetail>();

            if (type == MediaType.movie)
            {
                var result = await GetAsync<NewMovieData>(ImdbOptions.BaseUri + "ComingSoon".ConfigureParameters(parameter), true); //undefined numeric record

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
                        poster_small = ImdbOptions.ResizeImage + item.Image,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.movie
                    });
                }
            }
            else if (type == MediaType.tv)
            {
                throw new NotImplementedException();
            }

            return (list_media, true);
        }
    }
}
