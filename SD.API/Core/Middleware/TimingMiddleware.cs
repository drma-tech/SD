using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SD.API.Core.Middleware;

public class TimingMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var logger = context.InstanceServices?.GetService<ILoggerFactory>()?.CreateLogger("TimingMiddleware");

        var functionName = context.FunctionDefinition?.Name ?? "(unknown)";

        var sw = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();
            if (sw.ElapsedMilliseconds > 3000)
            {
                logger?.LogWarning("Function {FunctionName} executed in {ElapsedMilliseconds} ms", functionName, sw.Elapsed);
            }
        }
    }
}