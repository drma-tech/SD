using SD.Shared.Core;
using SD.Shared.Helper;
using SD.Shared.Modal.Tmdb;
using SD.WEB.Core;

namespace SD.WEB.Services
{
    public class TmdbExternalIdTvService
    {
        private readonly IConfiguration Configuration;

        public TmdbExternalIdTvService(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        public async Task<string> GetTmdbId(HttpClient http, IStorageService storage, Settings settings, string imdb_id)
        {
            var options = Configuration.GetSection(TmdbOptions.Section).Get<TmdbOptions>();

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", options.ApiKey },
                { "language", settings.Language.GetName() },
                { "external_source", "imdb_id" }
            };

            var result = await http.Get<FindByImdb>(storage.Session, options.BaseUri + "find/" + imdb_id.ConfigureParameters(parameter));

            return result.tv_results.First().id.ToString();
        }
    }
}