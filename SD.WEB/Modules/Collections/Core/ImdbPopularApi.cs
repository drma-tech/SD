using SD.Shared.Models.List.Imdb;
using SD.WEB.Modules.Collections.Interface;
using System.Globalization;

namespace SD.WEB.Modules.Collections.Core;

public class ImdbPopularApi(IHttpClientFactory factory)
    : ApiCosmos<CacheDocument<MostPopularData>>(factory, ApiType.Anonymous, null), IMediaListApi
{
    public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList,
        MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
    {
        var listMedia = new HashSet<MediaDetail>();

        if (type == MediaType.movie)
        {
            var result = await GetAsync("public/cache/imdb-popular-movies".ConfigureParameters(stringParameters), null);

            //if (!string.IsNullOrEmpty(result?.Data?.ErrorMessage)) throw new NotificationException(GlobalTranslations.UnavailableService);

            foreach (var item in result?.Data?.Items ?? [])
            {
                //if (item.IMDbRatingCount == "0") continue; //ignore low-rated movie
                //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                listMedia.Add(new MediaDetail
                {
                    tmdb_id = item.Id,
                    title = item.Title,
                    //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    release_date = string.IsNullOrEmpty(item.Year)
                        ? DateTime.MaxValue
                        : new DateTime(int.Parse(item.Year), 1, 1),
                    poster_small = item.Image,
                    rating = string.IsNullOrEmpty(item.IMDbRating)
                        ? 0
                        : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                    MediaType = MediaType.movie,
                    RankUpDown = item.RankUpDown
                });
            }
        }
        else if (type == MediaType.tv)
        {
            var result = await GetAsync("public/cache/imdb-popular-tvs".ConfigureParameters(stringParameters), null);

            //if (!string.IsNullOrEmpty(result?.Data?.ErrorMessage)) throw new NotificationException(GlobalTranslations.UnavailableService);

            foreach (var item in result?.Data?.Items ?? [])
            {
                //if (item.IMDbRatingCount == "0") continue; //ignore low-rated movie
                //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                //TODO: tv api has wrong poster definitions
                //var shortImage = !string.IsNullOrEmpty(item.Image) && item.Image.Contains("_V1_")
                //    ? item.Image?.Remove(item.Image.IndexOf("_V1_", StringComparison.Ordinal)) + "_V1_QL75_UY207_CR13,0,140,207_.jpg"
                //    : item.Image;

                listMedia.Add(new MediaDetail
                {
                    tmdb_id = item.Id,
                    title = item.Title,
                    //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    release_date = string.IsNullOrEmpty(item.Year)
                        ? DateTime.MaxValue
                        : new DateTime(int.Parse(item.Year), 1, 1),
                    poster_small = item.Image,
                    rating = string.IsNullOrEmpty(item.IMDbRating)
                        ? 0
                        : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                    MediaType = MediaType.tv,
                    RankUpDown = item.RankUpDown
                });
            }
        }
        else if (type == MediaType.person)
        {
            var result = await GetAsync("public/cache/imdb-popular-star".ConfigureParameters(stringParameters), null);

            foreach (var item in result?.Data?.Items ?? [])
            {
                listMedia.Add(new MediaDetail
                {
                    tmdb_id = item.Id,
                    title = item.Title,
                    //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    //release_date = null,
                    poster_small = item.Image,
                    //rating = 0,
                    MediaType = MediaType.person,
                    RankUpDown = item.RankUpDown
                });
            }
        }

        return (listMedia, true);
    }
}