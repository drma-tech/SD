using SD.Shared.Helper;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Tmdb;
using SD.WEB.Core;

namespace SD.WEB.Services.TMDB
{
    public static class WatchProvidersService
    {
        public static async Task<MediaProviders?> GetProviders(HttpClient http, IStorageService storage, Settings settings, string tmdb_id, MediaType type)
        {
            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey }
            };

            if (type == MediaType.movie)
            {
                return await http.Get<MediaProviders>(TmdbOptions.BaseUri + $"movie/{tmdb_id}/watch/providers".ConfigureParameters(parameter));
            }
            else //tv
            {
                return await http.Get<MediaProviders>(TmdbOptions.BaseUri + $"tv/{tmdb_id}/watch/providers".ConfigureParameters(parameter));
            }
        }
    }
}