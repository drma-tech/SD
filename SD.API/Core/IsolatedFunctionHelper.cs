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
        public static async Task<T> GetBody<T>(this HttpRequestData req, CancellationToken cancellationToken) where T : MainDocument, new()
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

            if (model is ProtectedMainDocument prot)
            {
                prot.Initialize(userId, userId);
            }
            else if (model is PrivateMainDocument priv)
            {
                priv.Initialize(userId);
            }

            return model;
        }

        public static async Task<T> GetPublicBody<T>(this HttpRequestData req, CancellationToken cancellationToken) where T : class, new()
        {
            req.Body.Position = 0; //in case of a previous read
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

        public static void ProcessException(this HttpRequestData req, Exception ex)
        {
            var logger = req.FunctionContext.GetLogger(req.FunctionContext.FunctionDefinition.Name);
            logger?.LogError(ex, req.BuildState(), req.BuildParams());
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

        public static string? BuildException(this Exception ex)
        {
            if (ex is CosmosException cex)
            {
                //TODO: review this

                //var result = JsonSerializer.Deserialize<CosmosExceptionStructure>("{" + cex.ResponseBody.Replace("Errors", "\"Errors\"") + "}", options: null);

                //return result?.Message?.Errors.FirstOrDefault();

                return cex.Message;
            }
            else
            {
                return ex.Message;
            }
        }
    }

    public class CosmosExceptionStructure
    {
        public string[] Errors { get; set; } = [];
    }

    public struct Method
    {
        public const string GET = "GET";

        public const string POST = "POST";

        public const string PUT = "PUT";

        public const string DELETE = "DELETE";
    }
}