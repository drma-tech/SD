using SD.Shared.Models.List.Tmdb;

namespace SD.WEB.Modules.Collections.Core;

public class TmdbCreditApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<Credits?> GetList(MediaType? type, string? tmdbId)
    {
        if (string.IsNullOrEmpty(tmdbId)) return null;

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey }
            //{ "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
        };

        if (type == MediaType.movie)
            return await GetAsync<Credits>(TmdbOptions.BaseUri + $"movie/{tmdbId}/credits".ConfigureParameters(parameter));

        return await GetAsync<Credits>(TmdbOptions.BaseUri + $"tv/{tmdbId}/credits".ConfigureParameters(parameter));
    }

    public async Task<CreditsByPerson?> GetListByPerson(string? personId)
    {
        if (string.IsNullOrEmpty(personId)) return null;

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", AppStateStatic.ContentLanguage.GetName(false) ?? "en-US" }
        };

        return await GetAsync<CreditsByPerson>(TmdbOptions.BaseUri + $"person/{personId}/combined_credits".ConfigureParameters(parameter));
    }
}