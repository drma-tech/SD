using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Support;
using SD.WEB.Shared;

namespace SD.WEB.Modules.Support.Core
{
    public class AnnouncementApi(IHttpClientFactory http, IMemoryCache memoryCache) : ApiCore<AnnouncementModel>(http, memoryCache, "AnnouncementModel")
    {
        private struct Endpoint
        {
            public const string Get = "Public/Announcements/Get";
        }

        public async Task<AnnouncementModel?> Get(RenderControlCore<AnnouncementModel?>? core)
        {
            return await GetAsync(Endpoint.Get, core);
        }
    }
}
