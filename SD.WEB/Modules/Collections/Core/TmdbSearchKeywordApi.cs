using SD.Shared.Models.List.Tmdb;

namespace SD.WEB.Modules.Collections.Core;

public class TmdbSearchKeywordApi(IHttpClientFactory factory) : ApiExternal(factory)
{
    public async Task<(HashSet<TmdbResultKeyword> list, bool lastPage)> GetKeywords(HashSet<TmdbResultKeyword> currentList, Dictionary<string, string>? stringParameters = null, int page = 1)
    {
        var parameter = new Dictionary<string, string>
        {
            { "api_key", TmdbOptions.ApiKey },
            { "page", page.ToString() },
        };

        if (stringParameters != null)
            foreach (var item in stringParameters)
                parameter.TryAdd(item.Key, item.Value);

        var result = await GetAsync<TmdbSearchKeyword>(TmdbOptions.BaseUri + "search/keyword".ConfigureParameters(parameter));

        if (result != null)
            foreach (var item in result.results)
            {
                currentList.Add(new TmdbResultKeyword
                {
                    id = item.id,
                    name = item.name,
                });
            }

        return new ValueTuple<HashSet<TmdbResultKeyword>, bool>(currentList, page >= result?.total_pages);
    }
}