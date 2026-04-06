using PosEvents.Api.Models;
using PosEvents.Api.Repository;

namespace PosEvents.Api.Tests.Repository;

public class InMemoryEventRepositoryTests
{
    [Fact]
    public async Task Add_then_GetAll_returns_the_event()
    {
        var repo = new InMemoryEventRepository();
        var posEvent = new PosEvent { EventType = "UserCreated" };

        await repo.AddEventAsync(posEvent);
        var events = await repo.GetAllEventsAsync();

        Assert.Single(events);
        Assert.Equal("UserCreated", events[0].EventType);
    }

    [Fact]
    public async Task GetAll_returns_empty_list_initially()
    {
        var repo = new InMemoryEventRepository();

        var events = await repo.GetAllEventsAsync();

        Assert.Empty(events);
    }

    [Fact]
    public async Task Add_multiple_events_preserves_all()
    {
        var repo = new InMemoryEventRepository();
        var e1 = new PosEvent { EventType = "A" };
        var e2 = new PosEvent { EventType = "B" };
        var e3 = new PosEvent { EventType = "C" };

        await repo.AddEventAsync(e1);
        await repo.AddEventAsync(e2);
        await repo.AddEventAsync(e3);
        var events = await repo.GetAllEventsAsync();

        Assert.Equal(3, events.Count);
        Assert.Equal("A", events[0].EventType);
        Assert.Equal("B", events[1].EventType);
        Assert.Equal("C", events[2].EventType);
    }

    [Fact]
    public async Task Concurrent_adds_do_not_corrupt_state()
    {
        var repo = new InMemoryEventRepository();

        var tasks = Enumerable.Range(0, 100)
            .Select(_ => repo.AddEventAsync(new PosEvent { EventType = "ConcurrentEvent" }));

        await Task.WhenAll(tasks);
        var events = await repo.GetAllEventsAsync();

        Assert.Equal(100, events.Count);
    }
}
