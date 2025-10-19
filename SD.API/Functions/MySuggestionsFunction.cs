using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace SD.API.Functions;

public class MySuggestionsFunction(CosmosRepository repo)
{
    [Function("MySuggestionsGet")]
    public async Task<HttpResponseData?> MySuggestionsGet(
        [HttpTrigger(AuthorizationLevel.User, Method.Get, Route = "my-suggestions/get")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var userId = req.GetUserId();

            var doc = await repo.Get<MySuggestions>(DocumentType.MySuggestions, userId, cancellationToken);

            return await req.CreateResponse(doc, TtlCache.OneDay, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("MySuggestionsSync")]
    public async Task<MySuggestions?> MySuggestionsSync(
        [HttpTrigger(AuthorizationLevel.User, Method.Post, Route = "my-suggestions/sync/{MediaType}")] HttpRequestData req, string mediaType, CancellationToken cancellationToken)
    {
        try
        {
            var userId = req.GetUserId();
            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("GetUserId null");

            var obj = await repo.Get<MySuggestions>(DocumentType.MySuggestions, userId, cancellationToken);
            var body = await req.GetPublicBody<MySuggestions>(cancellationToken);

            if (obj == null)
            {
                obj = body;

                obj.Initialize(userId);
            }
            else
            {
                obj = body;
            }

            var type = Enum.Parse<MediaType>(mediaType);

            if (type == MediaType.movie)
                obj.MovieSyncDate = DateTime.Now;
            else
                obj.ShowSyncDate = DateTime.Now;

            return await repo.UpsertItemAsync(obj, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    [Function("MySuggestionsAdd")]
    public async Task<MySuggestions?> MySuggestionsAdd(
        [HttpTrigger(AuthorizationLevel.User, Method.Post, Route = "my-suggestions/add")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var body = await req.GetBody<MySuggestions>(cancellationToken);

            return await repo.UpsertItemAsync(body, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }
}