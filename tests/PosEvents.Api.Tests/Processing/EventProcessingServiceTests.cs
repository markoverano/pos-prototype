using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PosEvents.Api.Models;
using PosEvents.Api.Observability;
using PosEvents.Api.Processing;
using System.Text.Json;

namespace PosEvents.Api.Tests.Processing;

public class EventProcessingServiceTests
{
    private readonly Mock<ILogger<EventProcessingService>> _loggerMock = new();
    private readonly EventProcessingService _sut;

    public EventProcessingServiceTests()
    {
        var metrics = new EventMetrics(NullLogger<EventMetrics>.Instance);
        _sut = new EventProcessingService(_loggerMock.Object, metrics);
    }

    private static PosEvent MakeEvent(string eventType, string? payloadJson = null)
    {
        JsonElement? payload = payloadJson is not null
            ? JsonDocument.Parse(payloadJson).RootElement.Clone()
            : null;

        return new PosEvent { EventType = eventType, Payload = payload };
    }

    private void VerifyLog(LogLevel level, string containsText, Times times)
    {
        _loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(containsText)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }

    [Fact]
    public async Task UserCreated_logs_account_initialized()
    {
        var posEvent = MakeEvent("UserCreated", """{"userId":"u1","email":"a@b.com"}""");

        await _sut.ProcessAsync(posEvent);

        VerifyLog(LogLevel.Information, "account initialized", Times.Once());
    }

    [Fact]
    public async Task OrderPlaced_logs_fulfillment_triggered()
    {
        var posEvent = MakeEvent("OrderPlaced", """{"orderId":"o1","items":[1,2,3]}""");

        await _sut.ProcessAsync(posEvent);

        VerifyLog(LogLevel.Information, "fulfillment triggered", Times.Once());
    }

    [Fact]
    public async Task Unknown_type_logs_warning_with_type_name()
    {
        var posEvent = MakeEvent("UnknownEventType");

        await _sut.ProcessAsync(posEvent);

        VerifyLog(LogLevel.Warning, "UnknownEventType", Times.Once());
    }

    [Fact]
    public async Task Processing_logs_start_and_completion()
    {
        var posEvent = MakeEvent("UnknownEventType");

        await _sut.ProcessAsync(posEvent);

        VerifyLog(LogLevel.Information, "Processing started", Times.Once());
        VerifyLog(LogLevel.Information, "Processing completed", Times.Once());
    }
}
