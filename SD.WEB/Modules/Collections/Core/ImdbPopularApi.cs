using SD.Shared.Models.List.Imdb;
using SD.WEB.Modules.Collections.Interface;
using System.Globalization;

namespace SD.WEB.Modules.Collections.Core;

public class ImdbPopularApi(IHttpClientFactory factory) : ApiCosmos<CacheDocument<MostPopularData>>(factory, ApiType.Anonymous, null, ApiContext.Default.CacheDocumentMostPopularData), IMediaListApi
{
    public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, ComponentActions<HashSet<MediaDetail>>? actions,
        MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1, CancellationToken cancellationToken = default)
    {
        var listMedia = new HashSet<MediaDetail>();
        if (actions != null && currentList.Empty()) await actions.StartLoading(null);

        if (type == MediaType.movie)
        {
            var result = await GetAsync("public/cache/imdb-popular-movies".ConfigureParameters(stringParameters), false, null, cancellationToken);

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
                    MediaType = MediaType.movie
                });
            }
        }
        else if (type == MediaType.tv)
        {
            var result = await GetAsync("public/cache/imdb-popular-tv".ConfigureParameters(stringParameters), false, null, cancellationToken);

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
                    MediaType = MediaType.tv
                });
            }
        }

        if (actions != null) await actions.FinishLoading(listMedia);

        return (listMedia, true);
    }
}