using System.Text.Json;

namespace PosEvents.Api.Models;

public class EventRequest
{
    public string? EventType { get; set; }
    public JsonElement? Payload { get; set; }
    public string? Timestamp { get; set; }
}
