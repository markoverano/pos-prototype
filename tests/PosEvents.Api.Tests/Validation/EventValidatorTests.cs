using PosEvents.Api.Models;
using PosEvents.Api.Validation;

namespace PosEvents.Api.Tests.Validation;

public class EventValidatorTests
{
    private readonly EventValidator _sut = new();

    [Fact]
    public void Valid_event_passes()
    {
        var request = new EventRequest
        {
            EventType = "UserCreated",
            Timestamp = DateTime.UtcNow.AddSeconds(-1).ToString("O")
        };

        var errors = _sut.Validate(request);

        Assert.Empty(errors);
    }

    [Fact]
    public void Missing_eventType_fails()
    {
        var request = new EventRequest
        {
            EventType = null,
            Timestamp = DateTime.UtcNow.AddSeconds(-1).ToString("O")
        };

        var errors = _sut.Validate(request);

        Assert.Single(errors);
        Assert.Equal("eventType", errors[0].Field);
    }

    [Fact]
    public void Empty_eventType_fails()
    {
        var request = new EventRequest
        {
            EventType = "",
            Timestamp = DateTime.UtcNow.AddSeconds(-1).ToString("O")
        };

        var errors = _sut.Validate(request);

        Assert.Single(errors);
        Assert.Equal("eventType", errors[0].Field);
    }

    [Fact]
    public void Future_timestamp_fails()
    {
        var request = new EventRequest
        {
            EventType = "UserCreated",
            Timestamp = DateTime.UtcNow.AddMinutes(10).ToString("O")
        };

        var errors = _sut.Validate(request);

        Assert.Single(errors);
        Assert.Equal("timestamp", errors[0].Field);
        Assert.Equal("timestamp must not be in the future.", errors[0].Message);
    }

    [Fact]
    public void Past_timestamp_passes()
    {
        var request = new EventRequest
        {
            EventType = "UserCreated",
            Timestamp = DateTime.UtcNow.AddHours(-1).ToString("O")
        };

        var errors = _sut.Validate(request);

        Assert.Empty(errors);
    }

    [Fact]
    public void Unparseable_timestamp_fails()
    {
        var request = new EventRequest
        {
            EventType = "UserCreated",
            Timestamp = "not-a-date"
        };

        var errors = _sut.Validate(request);

        Assert.Single(errors);
        Assert.Equal("timestamp", errors[0].Field);
        Assert.Equal("timestamp must be a valid ISO 8601 date-time string.", errors[0].Message);
    }

    [Fact]
    public void Missing_timestamp_fails()
    {
        var request = new EventRequest
        {
            EventType = "UserCreated",
            Timestamp = null
        };

        var errors = _sut.Validate(request);

        Assert.Single(errors);
        Assert.Equal("timestamp", errors[0].Field);
        Assert.Equal("timestamp must be a valid ISO 8601 date-time string.", errors[0].Message);
    }
}
