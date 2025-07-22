using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SD.Shared.Models.List.Tmdb;
using System.Diagnostics;
using System.Text.Json;

namespace SD.API.Functions;

public class TmdbFunction(IDistributedCache distributedCache, IHttpClientFactory factory)
{
    [Function("List4")]
    public async Task<HttpResponseData> List4(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/tmdb")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var client = factory.CreateClient("tmdb");
        var cacheKey = req.GetQueryParameters()["url"]?.ConvertFromBase64ToString() ?? throw new NotificationException("url null");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            CustomListNew? result;

            var cachedBytes = await distributedCache.GetAsync(cacheKey);
            if (cachedBytes is { Length: > 0 })
            {
                result = JsonSerializer.Deserialize<CustomListNew?>(cachedBytes);
            }
            else
            {
                result = await client.GetJsonFromApi<CustomListNew>(cacheKey, cancellationToken);
            }

            await SaveCache(result, cacheKey, TtlCache.OneDay);
            stopwatch.Stop();

            return await req.CreateResponse(result, TtlCache.OneDay, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            stopwatch.Stop();
            var logger = req.FunctionContext.GetLogger(req.FunctionContext.FunctionDefinition.Name);
            logger?.LogError(ex,
                "[ApiHelper] TaskCanceledException:\n" +
                "InvocationId={0}\n" +
                "TokenCanceled={1}\n" +
                "Timeout={2}s\n" +
                "Elapsed={3}ms\n" +
                "RequestUri={3}\n" +
                "StackTrace={4}",
                req.FunctionContext.InvocationId,
                cancellationToken.IsCancellationRequested,
                client.Timeout.TotalSeconds,
                stopwatch.ElapsedMilliseconds,
                cacheKey,
                ex.StackTrace
            );
            throw;
        }
        catch (OperationCanceledException ex)
        {
            stopwatch.Stop();
            var logger = req.FunctionContext.GetLogger(req.FunctionContext.FunctionDefinition.Name);
            logger?.LogError(ex,
               "[ApiHelper] OperationCanceledException:\n" +
               "InvocationId={0}\n" +
               "TokenCanceled={1}\n" +
               "Timeout={2}s\n" +
               "Elapsed={3}ms\n" +
               "RequestUri={3}\n" +
               "StackTrace={4}",
               req.FunctionContext.InvocationId,
               cancellationToken.IsCancellationRequested,
               client.Timeout.TotalSeconds,
               stopwatch.ElapsedMilliseconds,
               cacheKey,
               ex.StackTrace
            );
            throw;
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