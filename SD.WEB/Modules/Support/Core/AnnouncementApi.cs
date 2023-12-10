using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Support;

namespace SD.WEB.Modules.Support.Core
{
    public class AnnouncementApi(IHttpClientFactory http, IMemoryCache memoryCache) : ApiCore<AnnouncementModel>(http, memoryCache, "AnnouncementModel")
    {
        private struct Endpoint
        {
            public const string Get = "Public/Announcements/Get";
        }

        public async Task<AnnouncementModel?> Get()
        {
            return await GetAsync(Endpoint.Get);
        }
    }
}
