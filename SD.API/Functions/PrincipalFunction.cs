using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;
using SD.Shared.Model.Auth;

namespace SD.API.Functions
{
    public class PrincipalFunction
    {
        private readonly IRepository _repo;

        public PrincipalFunction(IRepository repo)
        {
            _repo = repo;
        }

        [Function("PrincipalGet")]
        public async Task<HttpResponseData> Get(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "Principal/Get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repo.Get<ClientePrincipal>(DocumentType.Principal + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                return await req.ProcessObject(result, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }

        [Function("PrincipalAdd")]
        public async Task<HttpResponseData> Add(
            [HttpTrigger(AuthorizationLevel.Function, Method.POST, Route = "Principal/Add")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var body = await req.GetBody<ClientePrincipal>(cancellationToken);

                var result = await _repo.Upsert(body, cancellationToken);

                return await req.ProcessObject(result, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }
    }
}