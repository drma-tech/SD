using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace SD.API.Core.Middleware;

internal sealed class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var functionName = context.FunctionDefinition.Name;
        var isTrailerFunction = functionName == "CacheTrailers";
        var stopwatch = isTrailerFunction ? System.Diagnostics.Stopwatch.StartNew() : null;

        if (isTrailerFunction)
            _logger.LogWarning("[Middleware] Início do processamento da request para {Function}", functionName);

        try
        {
            await next(context);
        }
        catch (CosmosOperationCanceledException ex)
        {
            _logger.LogError(ex, "CosmosOperationCanceledException");
            await context.SetHttpResponseStatusCode(HttpStatusCode.RequestTimeout, "Request Timeout!");
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "CosmosException");
            await context.SetHttpResponseStatusCode(HttpStatusCode.InternalServerError, "Invocation failed!");
        }
        catch (NotificationException ex)
        {
            await context.SetHttpResponseStatusCode(HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception");
            await context.SetHttpResponseStatusCode(HttpStatusCode.InternalServerError, "Invocation failed!");
        }
        finally
        {
            if (isTrailerFunction && stopwatch != null)
            {
                stopwatch.Stop();
                _logger.LogWarning("[Middleware] Fim do processamento da request para {Function}. Tempo total: {Elapsed}ms", functionName, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}