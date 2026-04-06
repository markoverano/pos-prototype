using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PosEvents.Api.Controllers;
using PosEvents.Api.Models;
using PosEvents.Api.Processing;
using PosEvents.Api.Repository;
using PosEvents.Api.Validation;

namespace PosEvents.Api.Tests.Controllers;

public class EventsControllerTests
{
    private readonly Mock<IEventRepository> _repoMock = new();
    private readonly Mock<IEventValidator> _validatorMock = new();
    private readonly Mock<IEventProcessor> _processorMock = new();
    private readonly Mock<ILogger<EventsController>> _loggerMock = new();
    private readonly EventsController _sut;

    public EventsControllerTests()
    {
        _processorMock
            .Setup(p => p.ProcessAsync(It.IsAny<PosEvent>()))
            .Returns(Task.CompletedTask);

        _sut = new EventsController(
            _repoMock.Object,
            _validatorMock.Object,
            _processorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Post_valid_event_returns_200_and_stores_event()
    {
        var request = new EventRequest
        {
            EventType = "UserCreated",
            Timestamp = DateTime.UtcNow.AddSeconds(-1).ToString("O")
        };

        _validatorMock
            .Setup(v => v.Validate(It.IsAny<EventRequest>()))
            .Returns(Array.Empty<ValidationError>());

        _repoMock
            .Setup(r => r.AddEventAsync(It.IsAny<PosEvent>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Post(request);

        Assert.IsType<OkResult>(result);
        _repoMock.Verify(r => r.AddEventAsync(It.IsAny<PosEvent>()), Times.Once);
    }

    [Fact]
    public async Task Post_invalid_event_returns_400_with_errors()
    {
        var request = new EventRequest { EventType = null, Timestamp = null };
        var errors = new List<ValidationError>
        {
            new() { Field = "eventType", Message = "eventType is required." },
            new() { Field = "timestamp", Message = "timestamp must be a valid ISO 8601 date-time string." }
        }.AsReadOnly();

        _validatorMock
            .Setup(v => v.Validate(It.IsAny<EventRequest>()))
            .Returns(errors);

        var result = await _sut.Post(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errors, badRequest.Value);
    }

    [Fact]
    public async Task Get_returns_200_with_all_events()
    {
        var events = new List<PosEvent>
        {
            new() { EventType = "UserCreated" },
            new() { EventType = "OrderPlaced" }
        }.AsReadOnly();

        _repoMock
            .Setup(r => r.GetAllEventsAsync())
            .ReturnsAsync((IReadOnlyList<PosEvent>)events);

        var result = await _sut.Get();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(events, ok.Value);
    }

    [Fact]
    public async Task Get_empty_returns_200_empty_collection()
    {
        _repoMock
            .Setup(r => r.GetAllEventsAsync())
            .ReturnsAsync((IReadOnlyList<PosEvent>)Array.Empty<PosEvent>());

        var result = await _sut.Get();

        var ok = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsAssignableFrom<IEnumerable<PosEvent>>(ok.Value);
        Assert.Empty(items);
    }
}
