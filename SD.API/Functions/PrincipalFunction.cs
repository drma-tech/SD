using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Support;

namespace SD.API.Functions
{
    public class PrincipalFunction(IRepository repo)
    {
        //[OpenApiOperation("PrincipalGet", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ClientePrincipal))]
        [Function("PrincipalGet")]
        public async Task<ClientePrincipal?> Get(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Principal/Get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

                return await repo.Get<ClientePrincipal>(DocumentType.Principal + ":" + userId, new PartitionKey(userId), cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("PrincipalGetEmail")]
        public async Task<string?> GetEmail(
          [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Public/Principal/GetEmail")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var token = req.GetQueryParameters()["token"];

                var principal = await repo.Get<ClientePrincipal>(DocumentType.Principal + ":" + token, new PartitionKey(token), cancellationToken);
                return principal?.Email;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        //[OpenApiOperation("PrincipalAdd", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ClientePrincipal))]
        [Function("PrincipalAdd")]
        public async Task<ClientePrincipal?> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "Principal/Add")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var body = await req.GetBody<ClientePrincipal>(cancellationToken);

                return await repo.Upsert(body, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("PrincipalPaddle")]
        public async Task<ClientePrincipal> Paddle(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.PUT, Route = "Principal/Paddle")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

                var Client = await repo.Get<ClientePrincipal>(DocumentType.Principal + ":" + userId, new PartitionKey(userId), cancellationToken) ?? throw new NotificationException("Client null");
                var body = await req.GetBody<ClientePrincipal>(cancellationToken);

                Client.ClientePaddle = body.ClientePaddle;

                return await repo.Upsert(Client, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("PrincipalRemove")]
        public async Task Remove(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.DELETE, Route = "Principal/Remove")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

                var myPrincipal = await repo.Get<ClientePrincipal>(DocumentType.Principal + ":" + userId, new PartitionKey(userId), cancellationToken);
                if (myPrincipal != null) await repo.Delete(myPrincipal, cancellationToken);

                var myProviders = await repo.Get<MyProviders>(DocumentType.MyProvider + ":" + userId, new PartitionKey(userId), cancellationToken);
                if (myProviders != null) await repo.Delete(myProviders, cancellationToken);

                var myLogins = await repo.Get<ClienteLogin>(DocumentType.Login + ":" + userId, new PartitionKey(userId), cancellationToken);
                if (myLogins != null) await repo.Delete(myLogins, cancellationToken);

                var mySuggestions = await repo.Get<MySuggestions>(DocumentType.MySuggestions + ":" + userId, new PartitionKey(userId), cancellationToken);
                if (mySuggestions != null) await repo.Delete(mySuggestions, cancellationToken);

                var myVotes = await repo.Query<TicketVoteModel>(x => x.IdVotedUser == userId, null, DocumentType.TicketVote, cancellationToken);
                foreach (var vote in myVotes)
                {
                    if (vote != null) await repo.Delete(vote, cancellationToken);
                }

                var myWatched = await repo.Get<WatchedList>(DocumentType.WatchedList + ":" + userId, new PartitionKey(userId), cancellationToken);
                if (myWatched != null) await repo.Delete(myWatched, cancellationToken);

                var myWatching = await repo.Get<WatchingList>(DocumentType.WatchingList + ":" + userId, new PartitionKey(userId), cancellationToken);
                if (myWatching != null) await repo.Delete(myWatching, cancellationToken);

                var myWish = await repo.Get<WishList>(DocumentType.WishList + ":" + userId, new PartitionKey(userId), cancellationToken);
                if (myWish != null) await repo.Delete(myWish, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}