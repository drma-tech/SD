using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SD.API.Core.Middleware;

internal sealed class TimingMiddleware(ILogger<TimingMiddleware> logger) : IFunctionsWorkerMiddleware
{
    private readonly ILogger<TimingMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
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
                _logger?.LogWarning("Function {FunctionName} executed in {ElapsedMilliseconds} ms", functionName, sw.Elapsed);
            }
        }
    }
}