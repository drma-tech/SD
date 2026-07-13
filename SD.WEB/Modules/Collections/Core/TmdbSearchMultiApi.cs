using SD.Shared.Models.List.Tmdb;
using SD.WEB.Modules.Collections.Interface;

namespace SD.WEB.Modules.Collections.Core;

public class TmdbSearchMultiApi(IHttpClientFactory factory) : ApiExternal(factory), IMediaListApi
{
    public async Task<(HashSet<MediaDetail> list, bool lastPage)> GetList(HashSet<MediaDetail> currentList, ComponentActions<HashSet<MediaDetail>>? actions,
        MediaType? type = null, Dictionary<string, string>? stringParameters = null, EnumLists? list = null, int page = 1, CancellationToken cancellationToken = default)
    {
        if (actions != null && currentList.Empty()) await actions.StartLoading(null);

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", (await AppStateStatic.GetContentLanguage(cancellationToken: cancellationToken)).GetFieldSettings(false).Name ?? "en-US" },
            { "page", page.ToString() },
            { "include_adult", "false" }
        };

        if (stringParameters != null)
            foreach (var item in stringParameters)
                parameter.TryAdd(item.Key, item.Value);

        var result = await GetAsync<TmdbSearchMulti>(TmdbOptions.BaseUri + "search/multi".ConfigureParameters(parameter), false, null, cancellationToken);

        if (result != null)
            foreach (var item in result.results.OrderByDescending(o => o.popularity))
            {
                if (item.media_type == "collection") continue;

                var mediaType = item.media_type.ParseToEnum<MediaType>();

                currentList.Add(new MediaDetail
                {
                    tmdb_id = item.id.ToString(),
                    title = mediaType == MediaType.movie ? item.title : item.name,
                    plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                    release_date = mediaType == MediaType.tv
                        ? item.first_air_date?.GetDate()
                        : item.release_date?.GetDate(),
                    poster_small = GetPoster(item, mediaType),
                    poster_large = string.IsNullOrEmpty(item.poster_path)
                        ? null
                        : TmdbOptions.LargePosterPath + item.poster_path,
                    rating = item.vote_count > 10 ? item.vote_average : 0,
                    MediaType = mediaType,
                    comments = GetComments(item, mediaType)
                });
            }

        if (actions != null) await actions.FinishLoading(currentList);
        return new ValueTuple<HashSet<MediaDetail>, bool>(currentList, page >= result?.total_pages);
    }

    private static string? GetPoster(TmdbResult? item, MediaType type)
    {
        if (item == null) return null;

        if (type == MediaType.person)
            return string.IsNullOrEmpty(item.profile_path) ? null : TmdbOptions.SmallPosterPath + item.profile_path;

        return string.IsNullOrEmpty(item.poster_path) ? null : TmdbOptions.SmallPosterPath + item.poster_path;
    }

    private static string? GetComments(TmdbResult? item, MediaType type)
    {
        if (item == null) return null;

        return type switch
        {
            MediaType.movie => MediaType.movie.GetFieldSettings().Name,
            MediaType.tv => MediaType.tv.GetFieldSettings().Name,
            MediaType.person => $"{MediaType.person.GetFieldSettings().Name},{item.known_for_department}",
            _ => null
        };
    }
}