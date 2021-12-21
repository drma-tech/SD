using SD.Shared.Helper;
using SD.Shared.Modal.Enum;
using SD.Shared.Modal.Tmdb;
using SD.WEB.Core;

namespace SD.WEB.Services.TMDB
{
    public class WatchProvidersService
    {
        private readonly IConfiguration Configuration;

        public WatchProvidersService(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        public async Task<MediaProviders> GetProviders(HttpClient http, IStorageService storage, Settings settings, string tmdb_id, MediaType type)
        {
            var options = Configuration.GetSection(TmdbOptions.Section).Get<TmdbOptions>();

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", options.ApiKey }
            };

            if (type == MediaType.movie)
            {
                return await http.Get<MediaProviders>(options.BaseUri + $"movie/{tmdb_id}/watch/providers".ConfigureParameters(parameter));
            }
            else //tv
            {
                return await http.Get<MediaProviders>(options.BaseUri + $"tv/{tmdb_id}/watch/providers".ConfigureParameters(parameter));
            }
        }
    }
}