using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Functions;

public class MyProvidersFunction(CosmosRepository repo)
{
    [Function("MyProviders")]
    public async Task<HttpResponseData?> MyProviders(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "my-providers")]
        HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var userId = req.GetUserId();

            var doc = await repo.Get<MyProviders>(DocumentType.MyProvider, userId, cancellationToken);

            return await req.CreateResponse(doc, TtlCache.OneDay, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("MyProvidersAdd")]
    public async Task<MyProviders?> MyProvidersAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "my-providers/add")]
        HttpRequestData req,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = req.GetUserId();
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<MyProviders>(DocumentType.MyProvider, userId, cancellationToken);

            if (obj == null)
            {
                obj = new MyProviders();

                obj.Initialize(userId);
            }

            var item = await req.GetPublicBody<MyProvidersItem>(cancellationToken);
            obj.AddItem([item]);

            return await repo.UpsertItemAsync(obj, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("MyProvidersUpdate")]
    public async Task<MyProviders?> MyProvidersUpdate(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "my-providers/update")]
        HttpRequestData req,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = req.GetUserId();
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<MyProviders>(DocumentType.MyProvider, userId, cancellationToken);

            if (obj == null) throw new InvalidOperationException("MyProviders null");

            var model = await req.GetPublicBody<MyProviders>(cancellationToken);

            return await repo.UpsertItemAsync(model, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("MyProvidersRemove")]
    public async Task<MyProviders?> MyProvidersRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "my-providers/remove")]
        HttpRequestData req,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = req.GetUserId();
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<MyProviders>(DocumentType.MyProvider, userId, cancellationToken);

            if (obj == null)
            {
                obj = new MyProviders();

                obj.Initialize(userId);
            }

            var item = await req.GetPublicBody<MyProvidersItem>(cancellationToken);
            obj.RemoveItem(item);

            return await repo.UpsertItemAsync(obj, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }
}