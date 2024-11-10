using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Functions
{
    public class MyProvidersFunction(CosmosRepository repo)
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
                return await repo.Get<MyProviders>(DocumentType.MyProvider, userId, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("MyProvidersAdd")]
        public async Task<MyProviders?> MyProvidersAdd(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "my-providers/add")] HttpRequestData req,
             CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<MyProviders>(DocumentType.MyProvider, userId, cancellationToken);

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
                throw;
            }
        }

        [Function("MyProvidersUpdate")]
        public async Task<MyProviders?> MyProvidersUpdate(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "my-providers/update")] HttpRequestData req,
             CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<MyProviders>(DocumentType.MyProvider, userId, cancellationToken);

                if (obj == null)
                {
                    throw new InvalidOperationException("MyProviders null");
                }
                else
                {
                    obj.Update();
                }

                var model = await req.GetPublicBody<MyProviders>(cancellationToken);

                return await repo.Upsert(model, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("MyProvidersRemove")]
        public async Task<MyProviders?> MyProvidersRemove(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "my-providers/remove")] HttpRequestData req,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

                var obj = await repo.Get<MyProviders>(DocumentType.MyProvider, userId, cancellationToken);

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
                throw;
            }
        }
    }
}