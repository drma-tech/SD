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
        }
    }
}