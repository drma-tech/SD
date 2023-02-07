using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;
using SD.Shared.Models.Support;

namespace SD.API.Functions
{
    public class SupportFunction
    {
        private readonly IRepository _repo;

        public SupportFunction(IRepository repo)
        {
            _repo = repo;
        }

        [Function("AnnouncementGetList")]
        public async Task<HttpResponseData> AnnouncementGetList(
           [HttpTrigger(AuthorizationLevel.Function, Method.GET, Route = "Public/Announcements/GetList")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repo.Query<AnnouncementModel>(null, null, DocumentType.Announcement, cancellationToken);

                return await req.ProcessObject(result, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("TicketGetList")]
        public async Task<HttpResponseData> TicketGetList(
            [HttpTrigger(AuthorizationLevel.Function, Method.GET, Route = "Public/Ticket/GetList")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repo.Query<TicketModel>(null, null, DocumentType.Ticket, cancellationToken);

                return await req.ProcessObject(result, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("TicketGetMyVotes")]
        public async Task<HttpResponseData> TicketGetMyVotes(
            [HttpTrigger(AuthorizationLevel.Function, Method.GET, Route = "Ticket/GetMyVotes")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repo.Query<TicketVoteModel>(x => x.IdVotedUser == req.GetUserId(), null, DocumentType.TicketVote, cancellationToken);

                return await req.ProcessObject(result, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("TicketInsert")]
        public async Task<HttpResponseData> TicketInsert(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "Ticket/Insert")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var item = await req.GetPublicBody<TicketModel>(cancellationToken);

                var result = await _repo.Upsert(item, cancellationToken);

                return await req.ProcessObject(result, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("TicketVote")]
        public async Task<HttpResponseData> TicketVote(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "Ticket/Vote")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var item = await req.GetPublicBody<TicketVoteModel>(cancellationToken);

                if (item.VoteType == VoteType.PlusOne)
                    await _repo.PatchItem<TicketModel>(nameof(DocumentType.Ticket) + ":" + item.Key, new PartitionKey(item.Key), new List<PatchOperation> { PatchOperation.Increment("/totalVotes", 1) }, cancellationToken);
                else if (item.VoteType == VoteType.MinusOne)
                    await _repo.PatchItem<TicketModel>(nameof(DocumentType.Ticket) + ":" + item.Key, new PartitionKey(item.Key), new List<PatchOperation> { PatchOperation.Increment("/totalVotes", -1) }, cancellationToken);

                item.IdVotedUser = req.GetUserId();

                var result = await _repo.Upsert(item, cancellationToken);

                return await req.ProcessObject(result, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }
    }
}