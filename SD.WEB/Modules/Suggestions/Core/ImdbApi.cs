using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.List.Imdb;

namespace SD.WEB.Modules.Suggestions.Core
{
    public class ImdbApi : ApiServices
    {
        public ImdbApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        public async Task<Ratings?> GetRatings(string? imdbId)
        {
            if (imdbId == null) return default;

            return await GetAsync<Ratings>(ImdbOptions.BaseUri + $"Ratings/{ImdbOptions.ApiKey}/{imdbId}", true);
        }
    }
}