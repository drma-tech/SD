using Microsoft.Extensions.Caching.Memory;
using SD.WEB.Modules.Suggestions.Interface;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class ImdbTopRatedApi : ApiServices, IMediaListApi
    {
        private readonly TmdbListApi _tmdbListApi;

        public ImdbTopRatedApi(IHttpClientFactory http, IMemoryCache memoryCache, TmdbListApi tmdbListApi) : base(http, memoryCache)
        {
            _tmdbListApi = tmdbListApi;
        }

        public Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            if (type == MediaType.movie)
            {
                return _tmdbListApi.GetList(currentList, type, stringParameters, EnumLists.ImdbTop250Movies, page);
            }
            else
            {
                return _tmdbListApi.GetList(currentList, type, stringParameters, EnumLists.ImdbTop250Shows, page);
            }

            //var parameter = new Dictionary<string, string>()
            //    {
            //        { "apiKey", ImdbOptions.ApiKey }
            //    };

            //var list_media = new HashSet<MediaDetail>();

            //if (type == MediaType.movie)
            //{
            //    var result = await GetAsync<Top250Data>(ImdbOptions.BaseUri + "Top250Movies".ConfigureParameters(parameter), true); //bring 250 records

            //    foreach (var item in result?.Items ?? new List<Top250DataDetail>())
            //    {
            //        //if (item.vote_count < 100) continue; //ignore low-rated movie
            //        //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
            //        if (new DateTime(int.Parse(item.Year ?? "0"), 1, 1) < DateTime.Now.AddYears(-20)) continue;

            //        list_media.Add(new MediaDetail
            //        {
            //            tmdb_id = item.Id,
            //            title = item.Title,
            //            //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
            //            release_date = new DateTime(int.Parse(item.Year ?? "0"), 1, 1),
            //            poster_small = ImdbOptions.ResizeImage + item.Image,
            //            rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
            //            MediaType = MediaType.movie
            //        });
            //    }
            //}
            //else if (type == MediaType.tv)
            //{
            //    var result = await GetAsync<Top250Data>(ImdbOptions.BaseUri + "Top250TVs".ConfigureParameters(parameter), true); //bring 250 records

            //    foreach (var item in result?.Items ?? new List<Top250DataDetail>())
            //    {
            //        //if (item.vote_count < 100) continue; //ignore low-rated movie
            //        //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
            //        if (new DateTime(int.Parse(item.Year ?? "0"), 1, 1) < DateTime.Now.AddYears(-20)) continue;

            //        list_media.Add(new MediaDetail
            //        {
            //            tmdb_id = item.Id,
            //            title = item.Title,
            //            //plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
            //            release_date = new DateTime(int.Parse(item.Year ?? "0"), 1, 1),
            //            poster_small = ImdbOptions.ResizeImage + item.Image,
            //            rating = string.IsNullOrEmpty(item.IMDbRating) ? 0 : double.Parse(item.IMDbRating, CultureInfo.InvariantCulture),
            //            MediaType = MediaType.tv
            //        });
            //    }
            //}

            //return list_media;
        }
    }
}