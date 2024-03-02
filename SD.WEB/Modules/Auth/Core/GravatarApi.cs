using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Core.Models;

namespace SD.WEB.Modules.Auth.Core
{
    public class GravatarApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiCore<GravatarRoot>(factory, memoryCache, "Gravatar", true)
    {
        public async Task<Gravatar?> GetGravatar(string? email)
        {
            if (string.IsNullOrEmpty(email)) return null;

            Gravatar? result;
            try
            {
                result = _cache.Get<Gravatar>("empty-gravatar");

                if (result == null)
                {
                    var root = await GetAsync($"https://en.gravatar.com/{email.GenerateHash()}.json", null);
                    result = root?.entry.LastOrDefault();
                }
            }
            catch (Exception)
            {
                result = new()
                {
                    displayName = email.Split("@")[0],
                    photos = [new Photo { value = $"https://en.gravatar.com/avatar/{email.GenerateHash()}?d=retro" }]
                };

                _cache.Set("empty-gravatar", result);
            }
            return result;
        }
    }
}