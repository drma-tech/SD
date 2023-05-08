using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Support;

namespace SD.WEB.Modules.Support.Core
{
    public class AnnouncementApi : ApiServices
    {
        public AnnouncementApi(HttpClient http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string Get = "Public/Announcements/Get";
        }

        public async Task<AnnouncementModel?> Get()
        {
            return await GetAsync<AnnouncementModel>(Endpoint.Get, false);
        }
    }
}
