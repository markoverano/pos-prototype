using System.Globalization;
using PosEvents.Api.Models;

namespace PosEvents.Api.Validation;

public class EventValidator : IEventValidator
{
    public IReadOnlyList<ValidationError> Validate(EventRequest request)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(request.EventType))
            errors.Add(new ValidationError { Field = "eventType", Message = "eventType is required." });

        if (string.IsNullOrWhiteSpace(request.Timestamp))
        {
            errors.Add(new ValidationError { Field = "timestamp", Message = "timestamp must be a valid ISO 8601 date-time string." });
        }
        else if (!DateTime.TryParse(
            request.Timestamp,
            null,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var parsedTimestamp))
        {
            errors.Add(new ValidationError { Field = "timestamp", Message = "timestamp must be a valid ISO 8601 date-time string." });
        }
        else if (parsedTimestamp > DateTime.UtcNow.AddSeconds(1))
        {
            errors.Add(new ValidationError { Field = "timestamp", Message = "timestamp must not be in the future." });
        }

        return errors.AsReadOnly();
    }
}
