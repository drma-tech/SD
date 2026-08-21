using SD.Shared.Models.List.Tmdb;
using SD.WEB.Api.Core;
using System.Globalization;

namespace SD.WEB.Api.Module.Cosmos.Anonymous;

public class TmdbListApi(IHttpClientFactory factory) : ApiExternal(factory), IMediaListApi
{
    public async Task<(ISet<MediaDetail> list, bool lastPage)> GetList(ISet<MediaDetail> currentList, RenderControlState<ISet<MediaDetail>>? actions,
        MediaType? type = null, IDictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1, CancellationToken cancellationToken = default)
    {
        if (list == null) throw new ArgumentException(message: null, nameof(list));
        if (actions != null && currentList.Empty()) await actions.StartLoading(null);

        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", (await AppStateStatic.GetContentLanguage(cancellationToken: cancellationToken)).GetFieldSettings(translate: false).Name ?? "en-US" },
            { "page", page.ToString(CultureInfo.InvariantCulture) },
        };

        var uri = $"{TmdbOptions.BaseUriNew}list/{((int)list).ToString(CultureInfo.InvariantCulture).ConfigureParameters(parameter)}";
        var result = await GetAsync<CustomListNew>(uri, setNewVersion: false, state: null, cancellationToken);

        if (result != null)
        {
            foreach (var item in result.results)
            {
                var tv = string.Equals(item.media_type, "tv", StringComparison.OrdinalIgnoreCase);

                string? value = null;
                result.comments?.TryGetValue(string.Create(CultureInfo.InvariantCulture, $"{(tv ? "tv" : "movie")}:{item.id}"), out value);

                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
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
                    comments = value,
                });
            }
        }

        if (actions != null) await actions.FinishLoading(currentList);
        return new ValueTuple<ISet<MediaDetail>, bool>(currentList, page >= result?.total_pages);
    }
}