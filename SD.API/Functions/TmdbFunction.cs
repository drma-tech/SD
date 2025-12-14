using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Caching.Distributed;
using SD.Shared.Models.List.Tmdb;
using System.Net;
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
            req.ValidateWebVersion();

            CustomListNew? result;

            var cacheKey = req.GetQueryParameters()["url"]?.ConvertFromBase64ToString() ?? throw new UnhandledException("url null");
            var cachedBytes = await distributedCache.GetAsync(cacheKey);
            if (cachedBytes is { Length: > 0 })
            {
                result = JsonSerializer.Deserialize<CustomListNew?>(cachedBytes);
            }
            else
            {
                var tmdbReadToken = ApiStartup.Configurations.TMDB?.ReadToken;
                var client = factory.CreateClient("tmdb");
                result = await client.GetdTmdbList<CustomListNew>(cacheKey, tmdbReadToken, cancellationToken);
            }

            await SaveCache(result, cacheKey, TtlCache.OneDay);

            return await req.CreateResponse(result, TtlCache.OneDay, cancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            req.LogError(ex.CancellationToken.IsCancellationRequested
                ? new NotificationException("Cancellation Requested")
                : new NotificationException("Timeout occurred"));

            return req.CreateResponse(HttpStatusCode.RequestTimeout);
        }
        catch (Exception ex)
        {
            req.LogError(ex);
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