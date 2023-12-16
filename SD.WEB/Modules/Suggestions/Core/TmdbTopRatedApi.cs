using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Suggestions.Interface;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class TmdbTopRatedApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiServices(factory, memoryCache), IMediaListApi
    {
        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "region", AppStateStatic.Region.ToString() },
                { "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
                { "page", page.ToString() }
            };

            if (type == MediaType.movie)
            {
                var result = await GetAsync<MovieTopRated>(TmdbOptions.BaseUri + "movie/top_rated".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? [])
                {
                    if (item.release_date?.GetDate() < DateTime.Now.AddYears(-20)) continue;
                    if (item.vote_count < 500) continue;
                    //if (string.IsNullOrEmpty(item.poster_path)) continue;

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_average,
                        MediaType = MediaType.movie
                    });
                }

                return new(currentList, page >= result?.total_pages);
            }
            else// if (type == MediaType.tv)
            {
                var result = await GetAsync<TVTopRated>(TmdbOptions.BaseUri + "tv/top_rated".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? [])
                {
                    if (item.first_air_date?.GetDate() < DateTime.Now.AddYears(-20)) continue;
                    if (item.vote_count < 500) continue;
                    if (string.IsNullOrEmpty(item.poster_path)) continue;

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.name,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = item.first_air_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_average,
                        MediaType = MediaType.tv
                    });
                }

                return new(currentList, page >= result?.total_pages);
            }
        }
    }
}