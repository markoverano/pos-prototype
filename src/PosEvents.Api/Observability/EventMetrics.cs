using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace PosEvents.Api.Observability;

public class EventMetrics
{
    private readonly ConcurrentDictionary<string, int> _countByType = new();
    private DateTime? _lastProcessedAt;
    private readonly ILogger<EventMetrics> _logger;

    public EventMetrics(ILogger<EventMetrics> logger)
    {
        _logger = logger;
    }

    public void RecordProcessed(string eventType, TimeSpan duration)
    {
        _countByType.AddOrUpdate(eventType, 1, (_, c) => c + 1);
        _lastProcessedAt = DateTime.UtcNow;

        _logger.LogInformation(
            "Metrics snapshot: EventType={EventType} DurationMs={DurationMs} Counts={Counts} LastProcessedAt={LastProcessedAt}",
            eventType, duration.TotalMilliseconds, GetCounts(), _lastProcessedAt);
    }

    public IReadOnlyDictionary<string, int> GetCounts() => _countByType;
    public DateTime? LastProcessedAt => _lastProcessedAt;
}
