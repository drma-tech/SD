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
                if (entry.Exception is null)
                {
                    logger.Log(entry.Level, "{Timestamp:o} {Message}", entry.Timestamp, entry.Message);
                }
                else
                {
                    logger.Log(entry.Level, entry.Exception, "{Timestamp:o} {Message}", entry.Timestamp, entry.Message);
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}