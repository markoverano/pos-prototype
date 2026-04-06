using System.Globalization;
using System.Text.Json;

namespace PosEvents.Api.Models;

public class PosEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string EventType { get; init; }
    public JsonElement? Payload { get; init; }
    public DateTime Timestamp { get; init; }
    public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;

    public static PosEvent From(EventRequest request) => new()
    {
        EventType = request.EventType!,
        Payload = request.Payload.HasValue && request.Payload.Value.ValueKind != JsonValueKind.Null
            ? request.Payload.Value.Clone()
            : null,
        Timestamp = DateTime.Parse(
            request.Timestamp!,
            null,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)
    };
}
