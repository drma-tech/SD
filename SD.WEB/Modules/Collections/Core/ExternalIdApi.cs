using SD.Shared.Models.List.Tmdb;
using System.Globalization;

namespace SD.WEB.Modules.Collections.Core;

public class ExternalIdApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<string?> GetTmdbId(MediaType? type, string? imdbId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(imdbId);

        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", (await AppStateStatic.GetContentLanguage(cancellationToken: cancellationToken)).GetFieldSettings(translate: false).Name ?? "en-US" },
            { "external_source", "imdb_id" },
        };

        var result = await GetAsync<FindByImdb>(TmdbOptions.BaseUri + $"find/{imdbId}".ConfigureParameters(parameter), setNewVersion: false, actions: null, cancellationToken);
        if (type == MediaType.movie)
            return result?.movie_results.FirstOrDefault()?.id.ToString(CultureInfo.InvariantCulture);

        if (type == MediaType.tv)
            return result?.tv_results.FirstOrDefault()?.id.ToString(CultureInfo.InvariantCulture);

        if (type == MediaType.person)
            return result?.person_results?.FirstOrDefault()?.id.ToString(CultureInfo.InvariantCulture);

        return null;
    }

    public async Task<string?> GetImdbId(MediaType? type, string? tmdbId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tmdbId);
        if (!type.HasValue)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", (await AppStateStatic.GetContentLanguage(cancellationToken: cancellationToken)).GetFieldSettings(translate: false).Name ?? "en-US" },
        };

        if (type == MediaType.movie)
        {
            var result = await GetAsync<MovieExternalIds>(TmdbOptions.BaseUri + $"movie/{tmdbId}/external_ids".ConfigureParameters(parameter), setNewVersion: false, actions: null, cancellationToken);

            return result?.imdb_id;
        }
        else
        {
            var result = await GetAsync<ShowExternalIds>(TmdbOptions.BaseUri + $"tv/{tmdbId}/external_ids".ConfigureParameters(parameter), setNewVersion: false, actions: null, cancellationToken);

            return result?.imdb_id;
        }
    }
}