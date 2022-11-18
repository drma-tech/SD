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
            public const string GetList = "Public/Announcements/GetList";
        }

        public async Task<HashSet<AnnouncementModel>> GetList()
        {
            return await GetListAsync<AnnouncementModel>(Endpoint.GetList, false);
        }
    }
}
