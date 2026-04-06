using PosEvents.Api.Models;

namespace PosEvents.Api.Repository;

public class InMemoryEventRepository : IEventRepository
{
    private readonly List<PosEvent> _events = [];
    private readonly object _lock = new();

    public Task AddEventAsync(PosEvent posEvent)
    {
        lock (_lock)
        {
            _events.Add(posEvent);
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<PosEvent>> GetAllEventsAsync()
    {
        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<PosEvent>>(_events.AsReadOnly());
        }
    }
}
