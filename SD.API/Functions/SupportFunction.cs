using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;
using SD.Shared.Models.Support;
using System.Net;

namespace SD.API.Functions
{
    public class SupportFunction
    {
        private readonly IRepository _repo;

        public SupportFunction(IRepository repo)
        {
            _repo = repo;
        }

        //[OpenApiOperation("AnnouncementGet", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(AnnouncementModel))]
        [Function("AnnouncementGet")]
        public async Task<AnnouncementModel?> AnnouncementGet(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Announcements/Get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                return await _repo.Get<AnnouncementModel>("Announcement", new PartitionKey("Announcement"), cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        //[OpenApiOperation("TicketGetList", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<TicketModel>))]
        [Function("TicketGetList")]
        public async Task<List<TicketModel>> TicketGetList(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Ticket/GetList")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                return await _repo.Query<TicketModel>(null, null, DocumentType.Ticket, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        //[OpenApiOperation("TicketGetMyVotes", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<TicketVoteModel>))]
        [Function("TicketGetMyVotes")]
        public async Task<List<TicketVoteModel>> TicketGetMyVotes(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Ticket/GetMyVotes")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

                return await _repo.Query<TicketVoteModel>(x => x.IdVotedUser == userId, null, DocumentType.TicketVote, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        //[OpenApiOperation("TicketInsert", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(TicketModel))]
        [Function("TicketInsert")]
        public async Task<TicketModel?> TicketInsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "Ticket/Insert")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var item = await req.GetPublicBody<TicketModel>(cancellationToken);

                return await _repo.Upsert(item, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        //[OpenApiOperation("TicketVote", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(TicketVoteModel))]
        [Function("TicketVote")]
        public async Task<TicketVoteModel?> TicketVote(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "Ticket/Vote")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var item = await req.GetPublicBody<TicketVoteModel>(cancellationToken);

                if (item.VoteType == VoteType.PlusOne)
                    await _repo.PatchItem<TicketModel>(nameof(DocumentType.Ticket) + ":" + item.Key, new PartitionKey(item.Key), new List<PatchOperation> { PatchOperation.Increment("/totalVotes", 1) }, cancellationToken);
                else if (item.VoteType == VoteType.MinusOne)
                    await _repo.PatchItem<TicketModel>(nameof(DocumentType.Ticket) + ":" + item.Key, new PartitionKey(item.Key), new List<PatchOperation> { PatchOperation.Increment("/totalVotes", -1) }, cancellationToken);

                item.IdVotedUser = req.GetUserId();

                return await _repo.Upsert(item, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}