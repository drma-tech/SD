using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;
using SD.Shared.Models.Auth;
using System.Net;

namespace SD.API.Functions
{
    public class PrincipalFunction
    {
        private readonly IRepository _repo;

        public PrincipalFunction(IRepository repo)
        {
            _repo = repo;
        }

        //[OpenApiOperation("PrincipalGet", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(ClientePrincipal))]
        [Function("PrincipalGet")]
        public async Task<ClientePrincipal?> Get(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Principal/Get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

                return await _repo.Get<ClientePrincipal>(DocumentType.Principal + ":" + userId, new PartitionKey(userId), cancellationToken);
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

                return await _repo.Upsert(body, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}