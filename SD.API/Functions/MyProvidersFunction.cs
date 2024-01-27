using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;

namespace SD.API.Functions
{
    public class MyProvidersFunction(IRepository repo)
    {
        //[OpenApiOperation("MyProviders", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(MyProviders))]
        [Function("MyProviders")]
        public async Task<MyProviders?> MyProviders(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "my-providers")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                return await repo.Get<MyProviders>(DocumentType.MyProvider + ":" + userId, new PartitionKey(userId), cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("MyProvidersAdd")]
        public async Task<MyProviders?> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "my-providers/add")] HttpRequestData req,
             CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<MyProviders>(DocumentType.MyProvider + ":" + userId, new PartitionKey(userId), cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(userId);
                }
                else
                {
                    obj.Update();
                }

                var item = await req.GetPublicBody<MyProvidersItem>(cancellationToken);
                obj.AddItem([item]);

                return await repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("MyProvidersRemove")]
        public async Task<MyProviders?> Remove(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "my-providers/remove")] HttpRequestData req,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<MyProviders>(DocumentType.MyProvider + ":" + userId, new PartitionKey(userId), cancellationToken);

                if (obj == null)
                {
                    obj = new();

                    obj.Initialize(userId);
                }
                else
                {
                    obj.Update();
                }

                var item = await req.GetPublicBody<MyProvidersItem>(cancellationToken);
                obj.RemoveItem(item);

                return await repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}