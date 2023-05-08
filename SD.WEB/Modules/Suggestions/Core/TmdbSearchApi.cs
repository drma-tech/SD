using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Suggestions.Interface;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class TmdbSearchApi : ApiServices, IMediaListApi
    {
        public TmdbSearchApi(IHttpClientFactory http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
                { "page", page.ToString() },
                { "include_adult", "false" }
            };

            if (stringParameters != null)
            {
                foreach (var item in stringParameters)
                {
                    parameter.TryAdd(item.Key, item.Value);
                }
            }

            var tv = type == MediaType.tv;
            var result = await GetByRequest<TmdbSearch>(TmdbOptions.BaseUri + $"search/{(tv ? "tv" : "movie")}".ConfigureParameters(parameter));

            if (result != null)
            {
                foreach (var item in result.results)
                {
                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = tv ? item.name : item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = tv ? item.first_air_date?.GetDate() : item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = tv ? MediaType.tv : MediaType.movie
                    });
                }
            }

            return new(currentList, page >= result?.total_pages);
        }
    }
}