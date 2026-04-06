using System.Text.Json;
using Microsoft.Extensions.Logging;
using PosEvents.Api.Models;

namespace PosEvents.Api.Processing.Handlers;

public class UserCreatedHandler
{
    private readonly ILogger _logger;

    public UserCreatedHandler(ILogger logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(PosEvent posEvent)
    {
        if (posEvent.Payload.HasValue)
        {
            var payload = posEvent.Payload.Value;

            var userId = payload.TryGetProperty("userId", out var uid) ? uid.GetString() : null;
            var email  = payload.TryGetProperty("email",  out var em)  ? em.GetString()  : null;

            _logger.LogInformation(
                "UserCreated payload: UserId={UserId} Email={Email}",
                userId, email);
        }

        _logger.LogInformation("UserCreated {EventId}: account initialized", posEvent.Id);

        await Task.Delay(50);
    }
}
