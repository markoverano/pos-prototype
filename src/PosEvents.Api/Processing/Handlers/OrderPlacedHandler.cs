using System.Text.Json;
using Microsoft.Extensions.Logging;
using PosEvents.Api.Models;

namespace PosEvents.Api.Processing.Handlers;

public class OrderPlacedHandler
{
    private readonly ILogger _logger;

    public OrderPlacedHandler(ILogger logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(PosEvent posEvent)
    {
        string? orderId = null;
        int itemCount = 0;
        string? totalAmount = null;

        if (posEvent.Payload.HasValue)
        {
            var payload = posEvent.Payload.Value;

            orderId = payload.TryGetProperty("orderId", out var oid) ? oid.GetString() : null;
            totalAmount = payload.TryGetProperty("totalAmount", out var ta) ? ta.GetRawText() : null;

            if (payload.TryGetProperty("items", out var items) &&
                items.ValueKind == JsonValueKind.Array)
            {
                itemCount = items.GetArrayLength();
            }
        }

        _logger.LogInformation(
            "OrderPlaced {EventId}: OrderId={OrderId} ItemCount={ItemCount} TotalAmount={TotalAmount}",
            posEvent.Id, orderId, itemCount, totalAmount);

        _logger.LogInformation("OrderPlaced {EventId}: fulfillment triggered", posEvent.Id);

        await Task.CompletedTask;
    }
}
