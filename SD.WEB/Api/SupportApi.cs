using Blazorise;
using SD.Shared.Model.Support;
using SD.WEB.Core;

namespace SD.WEB.Api
{
    public static class SupportApi
    {
        private struct SupportEndpoint
        {
            public const string GetList = "Public/Ticket/GetList";
            public const string GetMyVotes = "Ticket/GetMyVotes";

            public const string Insert = "Ticket/Insert";
            public const string Vote = "Ticket/Vote";
        }

        public static async Task<List<TicketModel>> Ticket_GetList(this HttpClient http)
        {
            return await http.GetList<TicketModel>(SupportEndpoint.GetList, false);
        }

        public static async Task<List<TicketVoteModel>> Ticket_GetMyVotes(this HttpClient http)
        {
            return await http.GetList<TicketVoteModel>(SupportEndpoint.GetMyVotes, false);
        }

        public static async Task Ticket_Insert(this HttpClient http, TicketModel obj, INotificationService toast)
        {
            var response = await http.Post(SupportEndpoint.Insert, false, obj);

            await response.ProcessResponse(toast, "Salvo com sucesso");
        }

        public static async Task Ticket_Vote(this HttpClient http, TicketVoteModel obj, INotificationService toast)
        {
            var response = await http.Post(SupportEndpoint.Vote, false, obj);

            await response.ProcessResponse(toast, "Voto registrado com sucesso");
        }
    }
}