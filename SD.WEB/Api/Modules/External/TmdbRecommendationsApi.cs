using SD.Shared.Models.List.Tmdb;
using SD.WEB.Api.Core;
using System.Globalization;

namespace SD.WEB.Api.Modules.External;

public class TmdbRecommendationsApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<IEnumerable<MediaDetail>> GetList(MediaType? type, string? tmdbId, RenderControlState<ICollection<MediaDetail>>? state, CancellationToken cancellationToken)
    {
        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", (await AppStateStatic.GetContentLanguage(cancellationToken: cancellationToken)).GetFieldSettings(translate: false).Name ?? "en-US" },
        };

        if (state != null) await state.StartLoading.Invoke(null);

        if (type == MediaType.movie)
        {
            return await ProcessMovie(tmdbId, state, parameter, cancellationToken);
        }
        else
        {
            return await ProcessTv(tmdbId, state, parameter, cancellationToken);
        }
    }

    private async Task<IEnumerable<MediaDetail>> ProcessMovie(string? tmdbId, RenderControlState<ICollection<MediaDetail>>? state, Dictionary<string, string> parameter, CancellationToken cancellationToken)
    {
        var result = await GetAsync<MoviePopular>(TmdbOptions.BaseUri3 + $"movie/{tmdbId}/recommendations".ConfigureParameters(parameter), setNewVersion: false, states: [], cancellationToken);

        var currentList = new List<MediaDetail>();

        foreach (var item in result?.results ?? [])
        {
            currentList.Add(new MediaDetail
            {
                tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
                title = item.title,
                plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                release_date = item.release_date?.GetDate(),
                poster_small = string.IsNullOrEmpty(item.poster_path)
                    ? null
                    : TmdbOptions.SmallPosterPath + item.poster_path,
                poster_large = string.IsNullOrEmpty(item.poster_path)
                    ? null
                    : TmdbOptions.LargePosterPath + item.poster_path,
                rating = item.vote_count > 10 ? item.vote_average : 0,
                MediaType = MediaType.movie,
            });
        }

        if (state != null) await state.FinishLoading.Invoke(currentList);

        return currentList;
    }

    private async Task<IEnumerable<MediaDetail>> ProcessTv(string? tmdbId, RenderControlState<ICollection<MediaDetail>>? state, Dictionary<string, string> parameter, CancellationToken cancellationToken)
    {
        var result = await GetAsync<TVPopular>(TmdbOptions.BaseUri3 + $"tv/{tmdbId}/recommendations".ConfigureParameters(parameter), setNewVersion: false, states: [], cancellationToken);

        var currentList = new List<MediaDetail>();

        foreach (var item in result?.results ?? [])
        {
            currentList.Add(new MediaDetail
            {
                tmdb_id = item.id.ToString(CultureInfo.InvariantCulture),
                title = item.name,
                plot = string.IsNullOrEmpty(item.overview) ? "No plot found" : item.overview,
                release_date = item.first_air_date?.GetDate(),
                poster_small = string.IsNullOrEmpty(item.poster_path)
                    ? null
                    : TmdbOptions.SmallPosterPath + item.poster_path,
                poster_large = string.IsNullOrEmpty(item.poster_path)
                    ? null
                    : TmdbOptions.LargePosterPath + item.poster_path,
                rating = item.vote_count > 10 ? item.vote_average : 0,
                MediaType = MediaType.tv,
            });
        }

        if (state != null) await state.FinishLoading.Invoke(currentList);

        return currentList;
    }
}