using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Tmdb;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class ExternalIdApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiServices(factory, memoryCache)
    {
        public async Task<string?> GetTmdbId(MediaType? type, string? imdb_id)
        {
            ArgumentNullException.ThrowIfNull(imdb_id);

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", AppStateStatic.Language.GetName(false) ?? "en-US" },
                { "external_source", "imdb_id" }
            };

            var result = await GetAsync<FindByImdb>(TmdbOptions.BaseUri + $"find/{imdb_id}".ConfigureParameters(parameter), true);
            if (type == MediaType.movie)
                return result?.movie_results.FirstOrDefault()?.id.ToString();
            else
                return result?.tv_results.FirstOrDefault()?.id.ToString();
        }

        public async Task<string?> GetImdbId(MediaType? type, string? tmdb_id)
        {
            if (tmdb_id == null) throw new ArgumentNullException(nameof(tmdb_id));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var parameter = new Dictionary<string, string>()
            {
                { "api_key", TmdbOptions.ApiKey },
                { "language", AppStateStatic.Language.GetName(false) ?? "en-US" }
            };

            if (type == MediaType.movie)
            {
                var result = await GetAsync<MovieExternalIds>(TmdbOptions.BaseUri + $"movie/{tmdb_id}/external_ids".ConfigureParameters(parameter), true);

                return result?.imdb_id;
            }
            else
            {
                var result = await GetAsync<ShowExternalIds>(TmdbOptions.BaseUri + $"tv/{tmdb_id}/external_ids".ConfigureParameters(parameter), true);

                return result?.imdb_id;
            }
        }
    }
}