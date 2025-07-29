using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace SD.API.Functions;

public class RedirectFunction
{
    [Function("RedirectFunction")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "suggestions/{slug}")] HttpRequestData req, string slug)
    {
        var response = req.CreateResponse(HttpStatusCode.MovedPermanently);
        var newUrl = $"/collections/{slug}";
        response.Headers.Add("Location", newUrl);
        return response;
    }
}