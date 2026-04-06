using PosEvents.Api.Models;

namespace PosEvents.Api.Processing;

public interface IEventProcessor
{
    Task ProcessAsync(PosEvent posEvent);
}
