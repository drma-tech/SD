using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SD.API.StartupLogging
{
    internal class StartupLogFlusher(ILogger<StartupLogFlusher> logger) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var entry in StartupLogBuffer.DequeueAll())
            {
                if (entry.TimeSpan.Milliseconds > 500)
                {
                    logger?.Log(LogLevel.Information, "Function {Method} executed in {TimeSpan} ({DateTime})", entry.Method, entry.TimeSpan, entry.DateTime);
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}