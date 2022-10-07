using Blazored.SessionStorage;
using SD.WEB.Core;

namespace SD.WEB.Services
{
    public class TmdbExternalIdTvService
    {
        public async Task<string> GetTmdbId(HttpClient Http, ISyncSessionStorageService session, string imdb_id)
        {
            return await Http.Get<string>($"TMDB/GetTmdbId/{imdb_id}", session);
        }
    }
}