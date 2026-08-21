using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using SD.Shared.Core.Types;

namespace SD.API.Functions.Auth;

public class MyProvidersFunction(CosmosMainRepository repo)
{
    [Function("MyProviders")]
    public async Task<HttpResponseData?> MyProviders(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "my-providers")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);

        var doc = await repo.ReadItemAsync<MyProviders>(new MainIdentity(MainType.MyProvider, userId), cancellationToken);

        return await req.CreateResponse(doc, TtlCache.OneDay, cancellationToken);
    }

    [Function("MyProvidersAdd")]
    public async Task<MyProviders?> MyProvidersAdd(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "my-providers/add")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);

        var obj = await repo.ReadItemAsync<MyProviders>(new MainIdentity(MainType.MyProvider, userId), cancellationToken);

        obj ??= new MyProviders(userId);

        var item = await req.GetBody<MyProvidersItem>(cancellationToken);
        obj.AddItem(new HashSet<MyProvidersItem>([item]));

        return await repo.UpsertItemAsync(obj);
    }

    [Function("MyProvidersUpdate")]
    public async Task<MyProviders?> MyProvidersUpdate(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "my-providers/update")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var body = await req.GetBody<MyProviders>(cancellationToken);
        await req.ValidateUser(body.Id, cancellationToken);
        return await repo.UpsertItemAsync(body);
    }

    [Function("MyProvidersRemove")]
    public async Task<MyProviders?> MyProvidersRemove(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "my-providers/remove")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var userId = await req.GetUserIdAsync(cancellationToken);

        var obj = await repo.ReadItemAsync<MyProviders>(new MainIdentity(MainType.MyProvider, userId), cancellationToken);

        obj ??= new MyProviders(userId);

        var item = await req.GetBody<MyProvidersItem>(cancellationToken);
        obj.RemoveItem(item);

        return await repo.UpsertItemAsync(obj);
    }
}