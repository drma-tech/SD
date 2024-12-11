using SD.Shared.Models.Support;
using SD.WEB.Shared;

namespace SD.WEB.Modules.Support.Core
{
    public class TicketApi(IHttpClientFactory http) : ApiCosmos<TicketModel>(http)
    {
        private struct Endpoint
        {
            public const string GetList = "public/ticket/get-list";
            public const string GetAll = "adm/ticket/get-all";
            public const string Insert = "Ticket/Insert";
        }

        public async Task<HashSet<TicketModel>> GetList(RenderControlCore<HashSet<TicketModel>>? core)
        {
            return await GetListAsync(Endpoint.GetList, core);
        }

        public async Task<HashSet<TicketModel>> GetAll(RenderControlCore<HashSet<TicketModel>>? core)
        {
            return await GetListAsync(Endpoint.GetAll, core);
        }

        public async Task<TicketModel?> Insert(TicketModel obj)
        {
            return await PostAsync(Endpoint.Insert, null, obj);
        }
    }
}