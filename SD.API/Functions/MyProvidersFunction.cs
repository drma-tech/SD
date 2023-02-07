using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;

namespace SD.API.Functions
{
    public class MyProvidersFunction
    {
        private readonly IRepository _repo;

        public MyProvidersFunction(IRepository repo)
        {
            _repo = repo;
        }

        [Function("MyProviders")]
        public async Task<HttpResponseData> MyProviders(
            [HttpTrigger(AuthorizationLevel.Function, Method.GET, Method.POST, Route = "MyProviders")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var model = await _repo.Get<MyProviders>(DocumentType.MyProvider + ":" + req.GetUserId(), new PartitionKey(req.GetUserId()), cancellationToken);

                if (req.Method == Method.POST)
                {
                    var body = await req.GetBody<MyProviders>(cancellationToken);

                    model ??= new();

                    model.Items = body.Items;
                    model = await _repo.Upsert(model, cancellationToken);
                }

                return await req.ProcessObject(model, cancellationToken);
            }
            catch (Exception ex)
            {
                return req.ProcessException(ex);
            }
        }
    }
}