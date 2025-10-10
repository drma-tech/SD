using SD.Shared.Models.List.Tmdb;

namespace SD.WEB.Modules.Collections.Core;

public class ExternalIdApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<string?> GetTmdbId(MediaType? type, string? imdbId)
    {
        ArgumentNullException.ThrowIfNull(imdbId);

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", (await AppStateStatic.GetContentLanguage()).GetName(false) ?? "en-US" },
            { "external_source", "imdb_id" }
        };

        var result = await GetAsync<FindByImdb>(TmdbOptions.BaseUri + $"find/{imdbId}".ConfigureParameters(parameter));
        if (type == MediaType.movie)
            return result?.movie_results.FirstOrDefault()?.id.ToString();
        else if (type == MediaType.tv)
            return result?.tv_results.FirstOrDefault()?.id.ToString();
        else if (type == MediaType.person)
            return result?.person_results?.FirstOrDefault()?.id.ToString();
        else
            return null;
    }

    public async Task<string?> GetImdbId(MediaType? type, string? tmdbId)
    {
        ArgumentNullException.ThrowIfNull(tmdbId);
        ArgumentNullException.ThrowIfNull(type);

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", (await AppStateStatic.GetContentLanguage()).GetName(false) ?? "en-US" }
        };

        if (type == MediaType.movie)
        {
            var result = await GetAsync<MovieExternalIds>(TmdbOptions.BaseUri + $"movie/{tmdbId}/external_ids".ConfigureParameters(parameter));

            return result?.imdb_id;
        }
        else
        {
            var result = await GetAsync<ShowExternalIds>(TmdbOptions.BaseUri + $"tv/{tmdbId}/external_ids".ConfigureParameters(parameter));

            return result?.imdb_id;
        }
    }
}