using SD.Shared.Models.List.Imdb;
using SD.WEB.Modules.Collections.Interface;
using System.Globalization;

namespace SD.WEB.Modules.Collections.Core;

public class ImdbPopularApi(IHttpClientFactory factory) : ApiCosmos<MostPopularDataCache>(factory, ApiType.Anonymous, null, [], ApiContext.Default.MostPopularDataCache), IMediaListApi
{
    public async Task<(ICollection<MediaDetail> list, bool lastPage)> GetList(ICollection<MediaDetail> currentList, RenderControlState<ICollection<MediaDetail>>? actions,
        MediaType? type = null, IDictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1, CancellationToken cancellationToken = default)
    {
        if (actions != null && currentList.Empty()) await actions.StartLoading(null);

        if (type == MediaType.movie)
        {
            var result = await GetAsync("public/cache/imdb-popular-movies".ConfigureParameters(stringParameters), setNewVersion: false, state: null, cancellationToken);

            foreach (var item in result?.Data?.Items ?? [])
            {
                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.Id,
                    title = item.Title,
                    //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    release_date = string.IsNullOrEmpty(item.Year)
                        ? DateTime.MaxValue
                        : new DateTime(int.Parse(item.Year, CultureInfo.InvariantCulture), 1, 1, 0, 0, 0, DateTimeKind.Local),
                    poster_small = item.Image,
                    rating = string.IsNullOrEmpty(item.IMDbRating)
                        ? 0
                        : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                    MediaType = MediaType.movie,
                });
            }
        }
        else if (type == MediaType.tv)
        {
            var result = await GetAsync("public/cache/imdb-popular-tv".ConfigureParameters(stringParameters), setNewVersion: false, state: null, cancellationToken);

            foreach (var item in result?.Data?.Items ?? [])
            {
                //TODO: tv api has wrong poster definitions
                //var shortImage = !string.IsNullOrEmpty(item.Image) && item.Image.Contains("_V1_")
                //    ? item.Image?.Remove(item.Image.IndexOf("_V1_", StringComparison.Ordinal)) + "_V1_QL75_UY207_CR13,0,140,207_.jpg"
                //    : item.Image;

                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.Id,
                    title = item.Title,
                    //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    release_date = string.IsNullOrEmpty(item.Year)
                        ? DateTime.MaxValue
                        : new DateTime(int.Parse(item.Year, CultureInfo.InvariantCulture), 1, 1, 0, 0, 0, DateTimeKind.Local),
                    poster_small = item.Image,
                    rating = string.IsNullOrEmpty(item.IMDbRating)
                        ? 0
                        : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
                    MediaType = MediaType.tv,
                });
            }
        }

        if (actions != null) await actions.FinishLoading(currentList);

        return (currentList, true);
    }
}