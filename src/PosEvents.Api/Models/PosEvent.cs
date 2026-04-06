using System.Text.Json;

namespace PosEvents.Api.Models;

public class PosEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string EventType { get; init; }
    public JsonElement? Payload { get; init; }
    public DateTime Timestamp { get; init; }
    public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;
}
