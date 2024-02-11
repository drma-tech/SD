using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SD.API.Core.Middleware
{
    internal sealed class AuthorizationMiddleware() : IFunctionsWorkerMiddleware
    {
        private static readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var req = await context.GetHttpRequestDataAsync() ?? throw new NotificationException("req null");

            var principal = new ClientPrincipal();

            if (req.Headers.TryGetValues("x-ms-client-principal", out var header))
            {
                var data = header.First();
                var decoded = Convert.FromBase64String(data);
                var json = Encoding.ASCII.GetString(decoded);
                principal = JsonSerializer.Deserialize<ClientPrincipal>(json, options) ?? throw new NotificationException("principal null");
            }

            if (req.Url.Host.Contains("localhost"))
            {
                principal = new ClientPrincipal { UserRoles = ["authenticated"] };
            }

            var targetMethod = GetTargetFunctionMethod(context);
            var attributes = targetMethod.GetCustomAttributes<AuthorizeAttribute>(true);

            if (attributes.Any() && principal.UserRoles.Empty())
            {
                await context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized, "Unauthorized");
            }

            if (attributes.Any() && !principal.UserRoles.Any(a => attributes.Single().Roles.Contains(a)))
            {
                await context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized, "Unauthorized");
            }

            await next(context);
        }

        public static MethodInfo GetTargetFunctionMethod(FunctionContext context)
        {
            var entryPoint = context.FunctionDefinition.EntryPoint;
            var assemblyPath = context.FunctionDefinition.PathToAssembly;
            var assembly = Assembly.LoadFrom(assemblyPath);
            var typeName = entryPoint.Substring(0, entryPoint.LastIndexOf('.'));
            var type = assembly.GetType(typeName);
            var methodName = entryPoint.Substring(entryPoint.LastIndexOf('.') + 1);
            var method = type.GetMethod(methodName);
            return method;
        }
    }
}