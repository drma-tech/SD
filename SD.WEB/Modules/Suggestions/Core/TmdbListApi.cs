using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Suggestions.Interface;

namespace SD.WEB.Modules.Suggestions.Core;

public class TmdbListApi(IHttpClientFactory factory) : ApiExternal(factory), IMediaListApi
{
    public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList,
        MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1)
    {
        if (list == null) throw new ArgumentException(null, nameof(list));

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
            { "page", page.ToString() }
        };

        var result = await GetAsync<CustomListNew>($"{BaseEndpoint}public/tmdb?url=" +
                        $"{TmdbOptions.BaseUriNew}list/{((int)list).ToString().ConfigureParameters(parameter)}".ConvertFromStringToBase64());

        if (result != null)
            foreach (var item in result.results)
            {
                var tv = item.media_type == "tv";

                result.comments.TryGetProperty($"{(tv ? "tv" : "movie")}:{item.id}", out var value);

                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = tv ? item.name : item.title,
                    plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    release_date = tv ? item.first_air_date?.GetDate() : item.release_date?.GetDate(),
                    poster_small = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.SmallPosterPath + item.poster_path,
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 10 ? item.vote_average : 0,
                    MediaType = tv ? MediaType.tv : MediaType.movie,
                    comments = value.GetString()
                });
            }

        return new ValueTuple<HashSet<MediaDetail>, bool>(currentList, page >= result?.total_pages);
    }
}