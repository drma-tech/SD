using Blazored.LocalStorage;
using SD.WEB.Core;

namespace SD.WEB.Services
{
    public class TmdbExternalIdTvService
    {
        public async Task<string> GetTmdbId(HttpClient Http, ISyncLocalStorageService session, string imdb_id)
        {
            return await Http.Get<string>(session, $"TMDB/GetTmdbId/{imdb_id}");
        }
    }
}