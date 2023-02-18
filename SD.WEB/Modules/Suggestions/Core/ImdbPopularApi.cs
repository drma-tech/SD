using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Model.List.Imdb;
using SD.Shared.Models.List.Imdb;
using SD.WEB.Modules.Suggestions.Interface;
using System.Globalization;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class ImdbPopularApi : ApiServices, IMediaListApi
    {
        public ImdbPopularApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            var list_media = new HashSet<MediaDetail>();

            if (type == MediaType.movie)
            {
                var result = await GetAsync<MostPopularDataCache>("Public/Cache/ImdbPopularMovies".ConfigureParameters(stringParameters), false);

                foreach (var item in result?.Data?.Items ?? new List<MostPopularDataDetail>())
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
                        release_date = new DateTime(int.Parse(item.Year ?? "0"), 1, 1),
                        poster_small = shortImage,
                        rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                        MediaType = MediaType.movie,
                        RankUpDown = item.RankUpDown,
                    });
                }
            }
            else if (type == MediaType.tv)
            {
                var result = await GetAsync<MostPopularDataCache>("Public/Cache/ImdbPopularTVs".ConfigureParameters(stringParameters), false);

                foreach (var item in result?.Data?.Items ?? new List<MostPopularDataDetail>())
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
                        release_date = new DateTime(int.Parse(item.Year ?? "0"), 1, 1),
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