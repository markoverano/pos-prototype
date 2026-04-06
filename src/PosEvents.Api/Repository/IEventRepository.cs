using PosEvents.Api.Models;

namespace PosEvents.Api.Repository;

public interface IEventRepository
{
    Task AddEventAsync(PosEvent posEvent);
    Task<IReadOnlyList<PosEvent>> GetAllEventsAsync();
}
