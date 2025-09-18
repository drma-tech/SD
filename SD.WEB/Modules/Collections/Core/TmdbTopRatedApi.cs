using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Collections.Interface;

namespace SD.WEB.Modules.Collections.Core;

public class TmdbTopRatedApi(IHttpClientFactory factory) : ApiExternal(factory), IMediaListApi
{
    public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList,
        MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null,
        int page = 1)
    {
        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "region", AppStateStatic.Region.ToString() },
            { "language", (await AppStateStatic.GetContentLanguage()).GetName(false) ?? "en-US" },
            { "page", page.ToString() }
        };

        if (type == MediaType.movie)
        {
            var result =
                await GetAsync<MovieTopRated>(TmdbOptions.BaseUri + "movie/top_rated".ConfigureParameters(parameter));

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
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_average,
                    MediaType = MediaType.movie
                });
            }

            return new ValueTuple<HashSet<MediaDetail>, bool>(currentList, page >= result?.total_pages);
        }
        else // if (type == MediaType.tv)
        {
            var result =
                await GetAsync<TVTopRated>(TmdbOptions.BaseUri + "tv/top_rated".ConfigureParameters(parameter));

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
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_average,
                    MediaType = MediaType.tv
                });
            }

            return new ValueTuple<HashSet<MediaDetail>, bool>(currentList, page >= result?.total_pages);
        }
    }
}