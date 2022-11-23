using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Model.List.Tmdb;
using SD.WEB.Modules.List.Interface;

namespace SD.WEB.Modules.List.Core
{
    public class TmdbUpcomingApi : ApiServices, IMediaListApi
    {
        public TmdbUpcomingApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "region", Settings.Region.ToString() },
                { "language", Settings.Language.GetName(false) ?? "en-US" },
                { "page", page.ToString() }
            };

            if (type == MediaType.movie)
            {
                var result = await GetAsync<MovieUpcoming>(TmdbOptions.BaseUri + "movie/upcoming".ConfigureParameters(parameter), true);

                foreach (var item in result?.results ?? new List<ResultMovieUpcoming>())
                {
                    //if (string.IsNullOrEmpty(item.poster_path)) continue;

                    currentList.Add(new MediaDetail
                    {
                        tmdb_id = item.id.ToString(),
                        title = item.title,
                        plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                        release_date = item.release_date?.GetDate(),
                        poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                        poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                        rating = item.vote_count > 10 ? item.vote_average : 0,
                        MediaType = MediaType.movie
                    });
                }

                return new(currentList, page >= result?.total_pages);
            }
            else// if (type == MediaType.tv)
            {
                throw new NotImplementedException();
            }
        }
    }
}
