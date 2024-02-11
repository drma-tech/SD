using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace SD.API.Core.Middleware
{
    public static class FunctionContextExtensions
    {
        public static async Task SetHttpResponseStatusCode(this FunctionContext context, HttpStatusCode statusCode, string? status)
        {
            var req = await context.GetHttpRequestDataAsync();

            var newHttpResponse = req.CreateResponse(statusCode);

            // You need to explicitly pass the status code in WriteAsJsonAsync method.
            // https://github.com/Azure/azure-functions-dotnet-worker/issues/776
            await newHttpResponse.WriteAsJsonAsync(new { Status = status }, newHttpResponse.StatusCode);

            var invocationResult = context.GetInvocationResult();

            var httpOutputBindingFromMultipleOutputBindings = GetHttpOutputBindingFromMultipleOutputBinding(context);
            if (httpOutputBindingFromMultipleOutputBindings is not null)
            {
                httpOutputBindingFromMultipleOutputBindings.Value = newHttpResponse;
            }
            else
            {
                invocationResult.Value = newHttpResponse;
            }
        }

        private static OutputBindingData<HttpResponseData>? GetHttpOutputBindingFromMultipleOutputBinding(FunctionContext context)
        {
            // The output binding entry name will be "$return" only when the function return type is HttpResponseData
            var httpOutputBinding = context.GetOutputBindings<HttpResponseData>()
                .FirstOrDefault(b => b.BindingType == "http" && b.Name != "$return");

            return httpOutputBinding;
        }
    }
}