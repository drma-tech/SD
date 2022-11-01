using SD.Shared.Model.List.Tmdb;

namespace SD.WEB.Modules.List.Core.TMDB
{
    public static class WatchProvidersService
    {
        public static async Task<MediaProviders?> GetProviders(HttpClient http, string? tmdb_id, MediaType? type)
        {
            if (tmdb_id == null) throw new ArgumentNullException(nameof(tmdb_id));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey }
            };

            if (type == MediaType.movie)
            {
                return await http.Get<MediaProviders>(TmdbOptions.BaseUri + $"movie/{tmdb_id}/watch/providers".ConfigureParameters(parameter), true);
            }
            else //tv
            {
                return await http.Get<MediaProviders>(TmdbOptions.BaseUri + $"tv/{tmdb_id}/watch/providers".ConfigureParameters(parameter), true);
            }
        }
    }
}