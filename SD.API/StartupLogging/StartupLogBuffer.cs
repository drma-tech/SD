using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SD.API.StartupLogging
{
    internal static class StartupLogBuffer
    {
        private static readonly ConcurrentQueue<LogEntry> _queue = new();

        public static void Enqueue(string message, Exception? exception = null)
        {
            _queue.Enqueue(new LogEntry
            {
                Level = LogLevel.Warning,
                Message = message,
                Exception = exception,
                Timestamp = DateTime.UtcNow
            });
        }

        public static IEnumerable<LogEntry> DequeueAll()
        {
            while (_queue.TryDequeue(out var entry))
            {
                yield return entry;
            }
        }

        internal class LogEntry
        {
            public LogLevel Level { get; set; }
            public string Message { get; set; } = string.Empty;
            public Exception? Exception { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}