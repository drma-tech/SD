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
        public async Task<MyProviders?> MyProviders(
            [HttpTrigger(AuthorizationLevel.Function, Method.GET, Method.POST, Route = "MyProviders")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                MyProviders? model;
                var userId = req.GetUserId();

                if (req.Method == Method.GET)
                {
                    model = await _repo.Get<MyProviders>(DocumentType.MyProvider + ":" + userId, new PartitionKey(userId), cancellationToken);
                }
                else
                {
                    model = await req.GetBody<MyProviders>(cancellationToken);
                    
                    model = await _repo.Upsert(model, cancellationToken);
                }

                return model;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}