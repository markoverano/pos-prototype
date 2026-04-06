using PosEvents.Api.Models;

namespace PosEvents.Api.Validation;

public interface IEventValidator
{
    IReadOnlyList<ValidationError> Validate(EventRequest request);
}
