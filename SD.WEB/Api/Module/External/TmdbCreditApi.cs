using SD.Shared.Models.List.Tmdb;
using SD.WEB.Api.Core;

namespace SD.WEB.Api.Module.External;

public class TmdbCreditApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<Credits?> GetList(MediaType? type, string? tmdbId, RenderControlState<Credits?>[] states, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(tmdbId)) return null;

        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
            //{ "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
        };

        if (type == MediaType.movie)
            return await GetAsync(TmdbOptions.BaseUri + $"movie/{tmdbId}/credits".ConfigureParameters(parameter), setNewVersion: false, states, cancellationToken);

        return await GetAsync(TmdbOptions.BaseUri + $"tv/{tmdbId}/credits".ConfigureParameters(parameter), setNewVersion: false, states, cancellationToken);
    }

    public async Task<CreditsByPerson?> GetListByPerson(string? personId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(personId)) return null;

        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", (await AppStateStatic.GetContentLanguage(cancellationToken: cancellationToken)).GetFieldSettings(translate: false).Name ?? "en-US" },
        };

        return await GetAsync<CreditsByPerson>(TmdbOptions.BaseUri + $"person/{personId}/combined_credits".ConfigureParameters(parameter), setNewVersion: false, states: [], cancellationToken);
    }
}