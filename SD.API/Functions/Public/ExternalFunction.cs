using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Auth;
using System.Net;

namespace SD.API.Functions.Public;

public class ExternalFunction(IHttpClientFactory factory)
{
    [Function("External")]
    public async Task<HttpResponseData> External(
        [HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/external")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var url = req.GetQueryParameter("url")?.ConvertFromBase64ToString();

        var allowedDomains = new[] { "api.themoviedb.org" };

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || !allowedDomains.Any(domain => string.Equals(uri.Host, domain, StringComparison.OrdinalIgnoreCase)))
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var client = factory.CreateClient("tmdb");

        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return await req.CreateResponse(stream, TtlCache.OneDay, isPublic: true, cancellationToken);
    }

    [Function("Country")]
    public async Task<HttpResponseData> Country([HttpTrigger(AuthorizationLevel.Anonymous, Method.Get, Route = "public/country")] HttpRequestData req, CancellationToken cancellationToken)
    {
        var ip = req.GetUserIP(includePort: false);

        var client = factory.CreateClient("ipinfo");

        if (ip.NotEmpty() && !string.Equals(ip, "127.0.0.1", StringComparison.Ordinal))
        {
            try
            {
                var result = await client.GetStringAsync($"https://ipinfo.io/{ip}/country", cancellationToken);
                return await req.CreateResponse(result.Trim().ToLowerInvariant(), TtlCache.OneMinute, isPublic: false, cancellationToken);
            }
            catch (Exception)
            {
                return await req.CreateResponse((string?)null, TtlCache.OneMinute, isPublic: false, cancellationToken);
            }
        }

        return await req.CreateResponse((string?)null, TtlCache.OneMinute, isPublic: false, cancellationToken);
    }
}
