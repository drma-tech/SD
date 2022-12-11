using Microsoft.Extensions.Caching.Memory;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class ExternalIdApi : ApiServices
    {
        public CacheSettings CacheSettings { get; set; } = new(TimeSpan.FromMinutes(10), TimeSpan.FromHours(1));

        public ExternalIdApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<string?> GetTmdbId(string? imdb_id)
        {
            if (imdb_id == null) throw new ArgumentNullException(nameof(imdb_id));

            return await GetAsync<string>($"TMDB/GetTmdbId/{imdb_id}", true, CacheSettings);
        }
    }
}