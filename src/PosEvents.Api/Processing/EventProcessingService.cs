using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PosEvents.Api.Models;
using PosEvents.Api.Observability;
using PosEvents.Api.Processing.Handlers;

namespace PosEvents.Api.Processing;

public class EventProcessingService : IEventProcessor
{
    private readonly Dictionary<string, Func<PosEvent, Task>> _handlers;
    private readonly ILogger<EventProcessingService> _logger;
    private readonly EventMetrics _metrics;

    public EventProcessingService(ILogger<EventProcessingService> logger, EventMetrics metrics)
    {
        _logger  = logger;
        _metrics = metrics;

        var userCreatedHandler  = new UserCreatedHandler(logger);
        var orderPlacedHandler  = new OrderPlacedHandler(logger);

        _handlers = new Dictionary<string, Func<PosEvent, Task>>(StringComparer.OrdinalIgnoreCase)
        {
            ["UserCreated"]  = userCreatedHandler.HandleAsync,
            ["OrderPlaced"]  = orderPlacedHandler.HandleAsync,
        };
    }

    public async Task ProcessAsync(PosEvent posEvent)
    {
        _logger.LogInformation(
            "Processing started: {EventType} {EventId}",
            posEvent.EventType, posEvent.Id);

        var sw = Stopwatch.StartNew();

        if (_handlers.TryGetValue(posEvent.EventType, out var handler))
            await handler(posEvent);
        else
            _logger.LogWarning(
                "No handler registered for event type: {EventType}",
                posEvent.EventType);

        sw.Stop();
        _metrics.RecordProcessed(posEvent.EventType, sw.Elapsed);

        _logger.LogInformation(
            "Processing completed: {EventType} {EventId} in {ElapsedMs}ms",
            posEvent.EventType, posEvent.Id, sw.ElapsedMilliseconds);
    }
}
