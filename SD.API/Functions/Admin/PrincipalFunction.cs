namespace SD.API.Functions.Admin;

public class PrincipalFunction(CosmosMainRepository repo)
{
    //[Function("PrincipalGetAll")]
    //public async Task<HttpResponseData?> PrincipalGetAll(
    //   [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "principal/get-all")] HttpRequestData req, CancellationToken cancellationToken)
    //{
    //    var data = await repo.ListAll<AuthPrincipal>(DocumentType.Principal, cancellationToken);

    //    return await req.CreateResponse(data, TtlCache.OneDay, cancellationToken);
    //}

    //[Function("PrincipalMigrate")]
    //public async Task PrincipalMigrate(
    //    [HttpTrigger(AuthorizationLevel.Anonymous, Method.Put, Route = "principal/migrate/{oldId}/{newId}")] HttpRequestData req, string oldId, string newId, CancellationToken cancellationToken)
    //{
    //    var myPrincipal = await repo.Get<AuthPrincipal>(DocumentType.Principal, oldId, cancellationToken);
    //    if (myPrincipal != null)
    //    {
    //        var model = myPrincipal.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myPrincipal, cancellationToken);
    //    }

    //    var myLogins = await repo.Get<AuthLogin>(DocumentType.Login, oldId, cancellationToken);
    //    if (myLogins != null)
    //    {
    //        var model = myLogins.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myLogins, cancellationToken);
    //    }

    //    var myProviders = await repo.Get<MyProviders>(DocumentType.MyProvider, oldId, cancellationToken);
    //    if (myProviders != null)
    //    {
    //        var model = myProviders.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myProviders, cancellationToken);
    //    }

    //    var mySuggestions = await repo.Get<MySuggestions>(DocumentType.MySuggestions, oldId, cancellationToken);
    //    if (mySuggestions != null)
    //    {
    //        var model = mySuggestions.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(mySuggestions, cancellationToken);
    //    }

    //    var myWatched = await repo.Get<WatchedList>(DocumentType.WatchedList, oldId, cancellationToken);
    //    if (myWatched != null)
    //    {
    //        var model = myWatched.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myWatched, cancellationToken);
    //    }

    //    var myWatching = await repo.Get<WatchingList>(DocumentType.WatchingList, oldId, cancellationToken);
    //    if (myWatching != null)
    //    {
    //        var model = myWatching.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myWatching, cancellationToken);
    //    }

    //    var myWish = await repo.Get<WishList>(DocumentType.WishList, oldId, cancellationToken);
    //    if (myWish != null)
    //    {
    //        var model = myWish.DeepClone();
    //        model.Initialize(newId);
    //        await repo.CreateItemAsync(model, cancellationToken);
    //        await repo.Delete(myWish, cancellationToken);
    //    }
    //}
}