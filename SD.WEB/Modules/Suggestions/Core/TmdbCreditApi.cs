using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Tmdb;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class TmdbCreditApi : ApiServices
    {
        public TmdbCreditApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<Credits?> GetList(MediaType? type, string? tmdb_id)
        {
            if (string.IsNullOrEmpty(tmdb_id)) return default;

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
            };

            if (type == MediaType.movie)
            {
                return await GetAsync<Credits>(TmdbOptions.BaseUri + $"movie/{tmdb_id}/credits".ConfigureParameters(parameter), true);
            }
            else
            {
                return await GetAsync<Credits>(TmdbOptions.BaseUri + $"tv/{tmdb_id}/credits".ConfigureParameters(parameter), true);
            }
        }
    }
}