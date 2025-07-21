using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SD.Shared.Models.List.Tmdb;
using System.Text.Json;

namespace SD.API.Functions;

public class TmdbFunction(IDistributedCache distributedCache, IHttpClientFactory factory)
{
    [Function("List4")]
    public async Task<HttpResponseData> List4(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/tmdb")] HttpRequestData req, CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = req.GetQueryParameters()["url"]?.ConvertFromBase64ToString() ?? throw new NotificationException("url null");

            CustomListNew? result;

            var cachedBytes = await distributedCache.GetAsync(cacheKey);
            if (cachedBytes is { Length: > 0 })
            {
                result = JsonSerializer.Deserialize<CustomListNew?>(cachedBytes);
            }
            else
            {
                var logger = req.FunctionContext.GetLogger(req.FunctionContext.FunctionDefinition.Name);
                var client = factory.CreateClient("tmdb");
                logger.LogWarning($"[List4] Timeout: {client.Timeout.TotalSeconds}s, CancellationToken: {cancellationToken.CanBeCanceled}, IsCancellationRequested: {cancellationToken.IsCancellationRequested}");
                result = await client.GetJsonFromApi<CustomListNew>(cacheKey, cancellationToken, logger);
            }

            await SaveCache(result, cacheKey, TtlCache.OneDay);

            return await req.CreateResponse(result, TtlCache.OneDay, cancellationToken);
        }
        catch (Exception ex)
        {
            req.ProcessException(ex);
            throw;
        }
    }

    private async Task SaveCache(CustomListNew? result, string cacheKey, TtlCache ttl)
    {
        if (result != null)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(result);
            await distributedCache.SetAsync(cacheKey, bytes, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds((int)ttl) });
        }
    }
}