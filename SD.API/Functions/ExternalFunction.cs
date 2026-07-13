using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;

namespace SD.API.Functions;

public class ExternalFunction(IHttpClientFactory factory)
{
    [Function("External")]
    public async Task<HttpResponseData> External(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/external")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var url = req.GetQueryParameters()["url"]?.ConvertFromBase64ToString() ?? throw new UnhandledException("url null");

        var client = factory.CreateClient("tmdb");

        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return await req.CreateResponse(stream, TtlCache.OneDay, cancellationToken);
    }

    [Function("Country")]
    public async Task<HttpResponseData> Country([HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/country")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var ip = req.GetUserIP(false);

        var client = factory.CreateClient("ipinfo");

        if (ip.NotEmpty() && ip != "127.0.0.1")
        {
            var result = await client.GetStringAsync($"https://ipinfo.io/{ip}/country", cancellationToken);
            return await req.CreateResponse(result, TtlCache.OneMinute, cancellationToken);
        }

        return await req.CreateResponse(null, TtlCache.OneMinute, cancellationToken);
    }
}