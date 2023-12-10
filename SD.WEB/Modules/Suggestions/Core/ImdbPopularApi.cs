using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Imdb;
using SD.WEB.Modules.Suggestions.Interface;
using System.Globalization;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class ImdbPopularApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiServices(factory, memoryCache), IMediaListApi
    {
        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            var list_media = new HashSet<MediaDetail>();

            if (type == MediaType.movie)
            {
                var result = await GetAsync<MostPopularData>("Public/Cache/ImdbPopularMovies".ConfigureParameters(stringParameters), false);

                //if (!string.IsNullOrEmpty(result?.Data?.ErrorMessage)) throw new NotificationException(GlobalTranslations.UnavailableService);

                foreach (var item in result?.Items ?? [])
                {
                    //if (item.IMDbRatingCount == "0") continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    //TODO: tv api has wrong poster definitions
                    var shortImage = !string.IsNullOrEmpty(item.Image) && item.Image.Contains("_V1_") ? item.Image?.Remove(item.Image.IndexOf("_V1_")) + "_V1_UX128_CR0,12,128,176_AL_.jpg" : item.Image;

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = string.IsNullOrEmpty(item.Year) ? DateTime.MaxValue : new DateTime(int.Parse(item.Year), 1, 1),
                        poster_small = shortImage,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.movie,
                        RankUpDown = item.RankUpDown,
                    });
                }
            }
            else if (type == MediaType.tv)
            {
                var result = await GetAsync<MostPopularData>("Public/Cache/ImdbPopularTVs".ConfigureParameters(stringParameters), false);

                //if (!string.IsNullOrEmpty(result?.Data?.ErrorMessage)) throw new NotificationException(GlobalTranslations.UnavailableService);

                foreach (var item in result?.Items ?? new List<MostPopularDataDetail>())
                {
                    //if (item.IMDbRatingCount == "0") continue; //ignore low-rated movie
                    //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                    //TODO: tv api has wrong poster definitions
                    var shortImage = !string.IsNullOrEmpty(item.Image) && item.Image.Contains("_V1_") ? item.Image?.Remove(item.Image.IndexOf("_V1_")) + "_V1_UX128_CR0,12,128,176_AL_.jpg" : item.Image;

                    list_media.Add(new MediaDetail
                    {
                        tmdb_id = item.Id,
                        title = item.Title,
                        //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = string.IsNullOrEmpty(item.Year) ? DateTime.MaxValue : new DateTime(int.Parse(item.Year), 1, 1),
                        poster_small = shortImage,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.tv,
                        RankUpDown = item.RankUpDown,
                    });
                }
            }

            return (list_media, true);
        }
    }
}