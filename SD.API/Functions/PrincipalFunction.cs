using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.Shared.Models.Auth;

namespace SD.API.Functions
{
    public class PrincipalFunction(CosmosRepository repo)
    {
        [Function("PrincipalGet")]
        public async Task<HttpResponseData?> PrincipalGet(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "principal/get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var doc = await repo.Get<ClientePrincipal>(DocumentType.Principal, userId, cancellationToken);

                return await req.CreateResponse(doc, ttlCache.one_day, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("PrincipalGetEmail")]
        public async Task<string?> PrincipalGetEmail(
          [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/principal/get-email")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var token = req.GetQueryParameters()["token"];

                var principal = await repo.Get<ClientePrincipal>(DocumentType.Principal, token, cancellationToken);

                return principal?.Email;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("PrincipalAdd")]
        public async Task<ClientePrincipal?> PrincipalAdd(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "principal/add")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var body = await req.GetBody<ClientePrincipal>(cancellationToken);

                return await repo.Upsert(body, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("PrincipalPaddle")]
        public async Task<ClientePrincipal> PrincipalPaddle(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.PUT, Route = "principal/paddle")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

                var Client = await repo.Get<ClientePrincipal>(DocumentType.Principal, userId, cancellationToken) ?? throw new NotificationException("Client null");
                var body = await req.GetBody<ClientePrincipal>(cancellationToken);

                Client.ClientePaddle = body.ClientePaddle;

                return await repo.Upsert(Client, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("PrincipalRemove")]
        public async Task PrincipalRemove(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.DELETE, Route = "principal/remove")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["offlineId"];

                var userId = id ?? req.GetUserId();

                var myPrincipal = await repo.Get<ClientePrincipal>(DocumentType.Principal, userId, cancellationToken);
                if (myPrincipal != null) await repo.Delete(myPrincipal, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }
    }
}