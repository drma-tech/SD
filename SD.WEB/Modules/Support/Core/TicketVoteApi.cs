using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Support;

namespace SD.WEB.Modules.Support.Core
{
    public class TicketVoteApi : ApiServices
    {
        public TicketVoteApi(IHttpClientFactory http, IMemoryCache memoryCache) : base(http, memoryCache)
        {
        }

        private struct Endpoint
        {
            public const string GetMyVotes = "Ticket/GetMyVotes";
            public const string Vote = "Ticket/Vote";
        }

        public async Task<HashSet<TicketVoteModel>> GetMyVotes()
        {
            return await GetListAsync<TicketVoteModel>(Endpoint.GetMyVotes, false);
        }

        public async Task<TicketVoteModel?> Vote(TicketVoteModel obj)
        {
            return await PostAsync(Endpoint.Vote, false, obj);
        }
    }
}