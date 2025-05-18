using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SD.API.Core;

public sealed class CosmosLoggerProvider(CosmosLogRepository repo) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, CosmosLogger> _loggers = new();
    private readonly CosmosLogRepository _repo = repo;

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new CosmosLogger(name, _repo));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}

public class CosmosLogger(string name, CosmosLogRepository repo) : ILogger
{
    private readonly string _name = name;
    private readonly CosmosLogRepository _repo = repo;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return default;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel.Warning;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        if (exception is NotificationException) return;

        _ = _repo.Add(new LogModel
        {
            Name = _name,
            State = formatter(state, exception),
            Message = exception?.Message,
            StackTrace = exception?.StackTrace,
            Ttl = (int)ttlCache.three_months
        });
    }
}