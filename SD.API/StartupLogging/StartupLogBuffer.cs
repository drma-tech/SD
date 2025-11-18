using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SD.API.StartupLogging
{
    internal static class StartupLogBuffer
    {
        private static readonly ConcurrentQueue<LogEntry> _queue = new();

        public static void Enqueue(string method, TimeSpan time)
        {
            _queue.Enqueue(new LogEntry
            {
                Method = method,
                DateTime = DateTime.UtcNow,
                TimeSpan = time
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
            public string? Method { get; set; }
            public DateTime DateTime { get; set; }
            public TimeSpan TimeSpan { get; set; }
        }
    }
}