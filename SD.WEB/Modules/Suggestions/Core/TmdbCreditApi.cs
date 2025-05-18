using SD.Shared.Models.List.Tmdb;

namespace SD.WEB.Modules.Suggestions.Core;

public class TmdbCreditApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<Credits?> GetList(MediaType? type, string? tmdb_id)
    {
        if (string.IsNullOrEmpty(tmdb_id)) return default;

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey }
            //{ "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
        };

        if (type == MediaType.movie)
            return await GetAsync<Credits>(TmdbOptions.BaseUri +
                                           $"movie/{tmdb_id}/credits".ConfigureParameters(parameter));

        return await GetAsync<Credits>(TmdbOptions.BaseUri + $"tv/{tmdb_id}/credits".ConfigureParameters(parameter));
    }

    public async Task<CreditsByPerson?> GetListByPerson(string? person_id)
    {
        if (string.IsNullOrEmpty(person_id)) return default;

        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "language", AppStateStatic.Language.GetName(false) ?? "en-US" }
        };

        return await GetAsync<CreditsByPerson>(TmdbOptions.BaseUri +
                                               $"person/{person_id}/combined_credits".ConfigureParameters(parameter));
    }
}