using Microsoft.Extensions.Caching.Memory;
using SD.WEB.Modules.Suggestions.Interface;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class ImdbTopRatedApi(IHttpClientFactory factory, IMemoryCache memoryCache, TmdbListApi tmdbListApi) : ApiServices(factory, memoryCache), IMediaListApi
    {
        public Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            if (type == MediaType.movie)
            {
                return tmdbListApi.GetList(currentList, type, stringParameters, EnumLists.ImdbTop250Movies, page);
            }
            else
            {
                return tmdbListApi.GetList(currentList, type, stringParameters, EnumLists.ImdbTop250Shows, page);
            }
        }
    }
}