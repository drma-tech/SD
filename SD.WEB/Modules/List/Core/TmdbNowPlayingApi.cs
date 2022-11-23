using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.List.Interface;

namespace SD.WEB.Modules.List.Core
{
    public class TmdbNowPlayingApi : ApiServices, IMediaListApi
    {
        public TmdbNowPlayingApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", Settings.Language.GetName(false) ?? "en-US" },
                { "region", Settings.Region.ToString() },
                { "page", page.ToString() }
            };

            var result = await GetAsync<MovieNowPlaying>(TmdbOptions.BaseUri + "movie/now_playing".ConfigureParameters(parameter), true);

            foreach (var item in result?.results ?? new List<ResultMovieNowPlaying>())
            {
                //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster
                if (item.release_date?.GetDate() > DateTime.Today.AddDays(1)) continue; //only accepts titles that will be released no later than one day after today

                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = item.title,
                    plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
                    release_date = item.release_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 5 ? item.vote_average : 0,
                    MediaType = MediaType.movie
                });
            }

            return new(currentList, page >= result?.total_pages);
        }
    }
}