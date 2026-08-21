using SD.Shared.Models.List.Tmdb;
using SD.WEB.Api.Core;
using System.Globalization;

namespace SD.WEB.Api.Module.External;

public class TmdbSearchKeywordApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<(IReadOnlyCollection<TmdbResultKeyword> list, bool lastPage)> GetKeywords(IReadOnlyCollection<TmdbResultKeyword> currentList, IReadOnlyDictionary<string, string>? stringParameters, int page, CancellationToken cancellationToken)
    {
        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
            { "page", page.ToString(CultureInfo.InvariantCulture) },
        };

        if (stringParameters != null)
            foreach (var item in stringParameters)
                parameter.TryAdd(item.Key, item.Value);

        var result = await GetAsync<TmdbSearchKeyword>(TmdbOptions.BaseUri + "search/keyword".ConfigureParameters(parameter), setNewVersion: false, states: [], cancellationToken);

        if (result != null)
            currentList = [.. result.results.Select(r => new TmdbResultKeyword
            {
                id = r.id,
                name = r.name,
            })];

        return new ValueTuple<IReadOnlyCollection<TmdbResultKeyword>, bool>(currentList, page >= result?.total_pages);
    }

    public async Task<IReadOnlyCollection<TmdbResultKeyword>> GetMovieKeywords(string? id, CancellationToken cancellationToken)
    {
        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
        };

        var result = await GetAsync<TmdbMovieKeyword>(TmdbOptions.BaseUri + $"movie/{id}/keywords".ConfigureParameters(parameter), setNewVersion: false, states: [], cancellationToken);

        return result?.keywords ?? [];
    }

    public async Task<IReadOnlyCollection<TmdbResultKeyword>> GetSerieKeywords(string? id, CancellationToken cancellationToken)
    {
        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "api_key", TmdbOptions.ApiKey },
        };

        var result = await GetAsync<TmdbSerieKeyword>(TmdbOptions.BaseUri + $"tv/{id}/keywords".ConfigureParameters(parameter), setNewVersion: false, states: [], cancellationToken);

        return result?.results ?? [];
    }
}