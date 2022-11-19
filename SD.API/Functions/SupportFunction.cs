using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SD.Shared.Models.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Functions
{
    public class SupportFunction
    {
        private readonly IRepository _repo;

        public SupportFunction(IRepository repo)
        {
            _repo = repo;
        }

        [FunctionName("AnnouncementGetList")]
        public async Task<IActionResult> AnnouncementGetList(
           [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.GET, Route = "Public/Announcements/GetList")] HttpRequest req,
           ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            try
            {
                var result = await _repo.Query<AnnouncementModel>(null, null, DocumentType.Announcement, cancellationToken);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("TicketGetList")]
        public async Task<IActionResult> TicketGetList(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.GET, Route = "Public/Ticket/GetList")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            try
            {
                var result = await _repo.Query<TicketModel>(null, null, DocumentType.Ticket, cancellationToken);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("TicketGetMyVotes")]
        public async Task<IActionResult> TicketGetMyVotes(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.GET, Route = "Ticket/GetMyVotes")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            try
            {
                var result = await _repo.Query<TicketVoteModel>(x => x.IdVotedUser == req.GetUserId(), null, DocumentType.TicketVote, cancellationToken);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("TicketInsert")]
        public async Task<IActionResult> TicketInsert(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "Ticket/Insert")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            try
            {
                var item = await req.GetParameterObjectPublic<TicketModel>(source.Token);

                var result = await _repo.Upsert(item, cancellationToken);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }

        [FunctionName("TicketVote")]
        public async Task<IActionResult> TicketVote(
            [HttpTrigger(AuthorizationLevel.Function, FunctionMethod.POST, Route = "Ticket/Vote")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, req.HttpContext.RequestAborted);

            try
            {
                var item = await req.GetParameterObjectPublic<TicketVoteModel>(source.Token);

                if (item.VoteType == VoteType.PlusOne)
                    await _repo.PatchItem<TicketModel>(nameof(DocumentType.Ticket) + ":" + item.Key, item.Key, new List<PatchOperation> { PatchOperation.Increment("/totalVotes", 1) }, cancellationToken);
                else if (item.VoteType == VoteType.MinusOne)
                    await _repo.PatchItem<TicketModel>(nameof(DocumentType.Ticket) + ":" + item.Key, item.Key, new List<PatchOperation> { PatchOperation.Increment("/totalVotes", -1) }, cancellationToken);
                
                item.SetIds(item.Key);

                var result = await _repo.Upsert(item, cancellationToken);

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, req.Query.BuildMessage(), req.Query.ToList());
                return new BadRequestObjectResult(ex.ProcessException());
            }
        }
    }
}