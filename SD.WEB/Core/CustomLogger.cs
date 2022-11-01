using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;

namespace SD.WEB.Core
{
    public sealed class CosmosLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, CustomLogger> _loggers = new();

        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, name => new CustomLogger(name));

        public void Dispose() => _loggers.Clear();
    }

    public class CustomLogger : ILogger
    {
        [Inject] protected ISyncSessionStorageService Session { get; set; } = default!;

        private readonly string _name;

        public CustomLogger(string name)
        {
            _name = name;
        }

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (exception is NotificationException)
            {
                return;
            }

            var list = new List<LogContainer>();

            if (Session != null && Session.ContainKey("LogErrosVD"))
            {
                list = Session.GetItem<List<LogContainer>>("LogErrosVD");
            }

            list.Add(new LogContainer()
            {
                Name = _name,
                State = formatter(state, exception),
                Message = exception?.Message,
                StackTrace = exception?.StackTrace
            });

            Session?.SetItem("LogErrosVD", list);
        }
    }

    public class LogContainer
    {
        public string? Name { get; set; }
        public string? State { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
    }
}