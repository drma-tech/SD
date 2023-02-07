using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SD.API.Core
{
    public class CosmosLogger : ILogger
    {
        private readonly string _name;
        private readonly CosmosLogRepository _repo;

        public CosmosLogger(string name, CosmosLogRepository repo)
        {
            _name = name;
            _repo = repo;
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (exception is NotificationException)
            {
                return;
            }

            _ = _repo.Add(new LogModel()
            {
                Name = _name,
                State = formatter(state, exception),
                Message = exception?.Message,
                StackTrace = exception?.StackTrace
            });
        }
    }

    public sealed class CosmosLoggerProvider : ILoggerProvider
    {
        private readonly CosmosLogRepository _repo;
        private readonly ConcurrentDictionary<string, CosmosLogger> _loggers = new();

        public CosmosLoggerProvider(CosmosLogRepository repo)
        {
            _repo = repo;
        }

        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, name => new CosmosLogger(name, _repo));

        public void Dispose() => _loggers.Clear();
    }
}