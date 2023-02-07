using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Text.Json;
using System.Web;

namespace SD.API.Core
{
    public static class IsolatedFunctionHelper
    {
        public static async Task<T> GetBody<T>(this HttpRequestData req, CancellationToken cancellationToken) where T : CosmosDocument, new()
        {
            var model = await JsonSerializer.DeserializeAsync<T>(req.Body, cancellationToken: cancellationToken);

            if (model == null)
            {
                model = new T();
            }
            else
            {
                model.Update();
            }

            var userId = req.GetUserId();

            if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("unauthenticated user");

            model.SetIds(userId, userId);

            return model;
        }

        public static async Task<T> GetPublicBody<T>(this HttpRequestData req, CancellationToken cancellationToken) where T : class, new()
        {
            var model = await JsonSerializer.DeserializeAsync<T>(req.Body, cancellationToken: cancellationToken);
            //TODO: call Update
            model ??= new T();

            return model;
        }

        public static StringDictionary GetQueryParameters(this HttpRequestData req)
        {
            var valueCollection = HttpUtility.ParseQueryString(req.Url.Query);

            var dictionary = new StringDictionary();
            foreach (var key in valueCollection.AllKeys)
            {
                if (key != null)
                {
                    dictionary.Add(key.ToLowerInvariant(), valueCollection[key]);
                }
            }
            return dictionary;
        }

        public static HttpResponseData ProcessException(this HttpRequestData req, Exception ex)
        {
            //logger
            var logger = req.FunctionContext.GetLogger(req.FunctionContext.FunctionDefinition.Name);
            logger?.LogError(ex, req.BuildState(), req.BuildParams());

            //response data
            var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            errorResponse.WriteString(ex.BuildException() ?? "");
            return errorResponse;
        }

        public static async Task<HttpResponseData> ProcessObject<T>(this HttpRequestData req, T instance, CancellationToken cancellationToken)
        {
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteAsJsonAsync(instance, cancellationToken);
            return response;
        }

        private static string[] BuildParams(this HttpRequestData req)
        {
            var valueCollection = HttpUtility.ParseQueryString(req.Url.Query);

            return valueCollection.AllKeys.Select((key) => valueCollection[key] ?? "").ToArray();
        }

        private static string BuildState(this HttpRequestData req)
        {
            var valueCollection = HttpUtility.ParseQueryString(req.Url.Query);

            return string.Join("", valueCollection.AllKeys.Select((key) => $"{key?.ToLowerInvariant()}={{{key?.ToLowerInvariant()}}}|"));
        }

        private static string? BuildException(this Exception ex)
        {
            if (ex is CosmosException cex)
            {
                //var result = JsonSerializer.Deserialize<CosmosExceptionStructure>(cex.ResponseBody);
                var result = JsonSerializer.Deserialize<CosmosExceptionStructure>("{" + cex.ResponseBody.Replace("Errors", "\"Errors\"") + "}", options: null);

                return result?.Errors.FirstOrDefault();
            }
            else
            {
                return ex.Message;
            }
        }
    }

    public class CosmosExceptionStructure
    {
        public string[] Errors { get; set; } = Array.Empty<string>();
    }

    public struct Method
    {
        public const string GET = "GET";

        public const string POST = "POST";

        public const string PUT = "PUT";

        public const string DELETE = "DELETE";
    }
}