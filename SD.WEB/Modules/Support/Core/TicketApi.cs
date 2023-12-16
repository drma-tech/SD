using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Support;
using SD.WEB.Shared;

namespace SD.WEB.Modules.Support.Core
{
    public class TicketApi(IHttpClientFactory http, IMemoryCache memoryCache) : ApiCore<TicketModel>(http, memoryCache, "TicketModel")
    {
        private struct Endpoint
        {
            public const string GetList = "Public/Ticket/GetList";
            public const string Insert = "Ticket/Insert";
        }

        public async Task<HashSet<TicketModel>> GetList(RenderControlCore<HashSet<TicketModel>>? core)
        {
            return await GetListAsync(Endpoint.GetList, core);
        }

        public async Task<TicketModel?> Insert(TicketModel obj)
        {
            return await PostAsync(Endpoint.Insert,null, obj);
        }
    }
}