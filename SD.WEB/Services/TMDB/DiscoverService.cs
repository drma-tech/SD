using SD.Shared.Core;
using SD.Shared.Helper;
using SD.Shared.Modal;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Tmdb;
using SD.WEB.Core;

namespace SD.WEB.Services.TMDB
{
    public class DiscoverService : IMediaListService
    {
        public async Task PopulateListMedia(HttpClient http, IStorageService storage, Settings settings,
            HashSet<MediaDetail> list_media, MediaType type, int qtd = 9, Dictionary<string, string>? ExtraParameters = null)
        {
            var page = 0;

            if (ExtraParameters != null)
            {
                if (ExtraParameters.ContainsValue("popularity.desc"))
                {
                    ExtraParameters.TryAdd("vote_count.gte", "50"); //ignore low-rated movie
                }
                if (ExtraParameters.ContainsValue("primary_release_date.desc"))
                {
                    ExtraParameters.TryAdd("primary_release_date.lte", DateTime.Now.ToString("yyyy-MM-dd")); //only releasead
                }
                if (ExtraParameters.ContainsValue("vote_average.desc"))
                {
                    ExtraParameters.TryAdd("primary_release_date.gte", DateTime.Now.AddYears(-20).ToString("yyyy-MM-dd")); //only recent releases
                    ExtraParameters.TryAdd("vote_count.gte", "1000"); //ignore low-rated movie
                    ExtraParameters.TryAdd("vote_average.gte", "7.4"); //only the best
                }
            }

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", settings.Language.GetName(false) ?? "en-US" },
                { "watch_region", settings.Region.ToString() },
                { "page", page.ToString() }
            };

            if (ExtraParameters != null)
            {
                foreach (var item in ExtraParameters)
                {
                    parameter.TryAdd(item.Key, item.Value);
                }
            }

            if (type == MediaType.movie)
            {
                while (list_media.Count < qtd)
                {
                    page++;
                    parameter["page"] = page.ToString();
                    var result = await http.Get<MovieDiscover>(TmdbOptions.BaseUri + "discover/movie".ConfigureParameters(parameter), storage.Session);

                    foreach (var item in result?.results ?? new List<ResultMovieDiscover>())
                    {
                        //if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                        list_media.Add(new MediaDetail
                        {
                            tmdb_id = item.id.ToString(),
                            title = item.title,
                            plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
                            release_date = item.release_date?.GetDate(),
                            poster_path_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                            poster_path_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                            rating = item.vote_count > 5 ? item.vote_average : 0,
                            MediaType = MediaType.movie
                        });
                    }

                    if (result?.total_results < qtd) break; //if there is less result than requested
                    if (page >= result?.total_pages) break; //passed the last page
                    if (page > 10) break; //if it exceeds 10 calls, something is wrong
                }
            }
            else if (type == MediaType.tv)
            {
                while (list_media.Count < qtd)
                {
                    page++;
                    parameter["page"] = page.ToString();
                    var result = await http.Get<TvDiscover>(TmdbOptions.BaseUri + "discover/tv".ConfigureParameters(parameter), storage.Session);

                    foreach (var item in result?.results ?? new List<ResultTvDiscover>())
                    {
                        if (string.IsNullOrEmpty(item.poster_path)) continue; //ignore empty poster

                        list_media.Add(new MediaDetail
                        {
                            tmdb_id = item.id.ToString(),
                            title = item.name,
                            plot = string.IsNullOrEmpty(item.overview) ? SD.Shared.Resources.TranslationText.NoPlot : item.overview,
                            release_date = item.first_air_date?.GetDate(),
                            poster_path_small = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path,
                            poster_path_large = string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.LargePosterPath + item.poster_path,
                            rating = item.vote_count > 10 ? item.vote_average : 0,
                            MediaType = MediaType.tv
                        });
                    }

                    if (result?.total_results < qtd) break; //if there is less result than requested
                    if (page >= result?.total_pages) break; //passed the last page
                    if (page > 10) break; //if it exceeds 10 calls, something is wrong
                }
            }
        }
    }
}