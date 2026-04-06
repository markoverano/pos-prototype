using Microsoft.AspNetCore.Mvc;
using PosEvents.Api.Models;
using PosEvents.Api.Processing;
using PosEvents.Api.Repository;
using PosEvents.Api.Validation;

namespace PosEvents.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _repository;
    private readonly IEventValidator _validator;
    private readonly IEventProcessor _processor;
    private readonly ILogger<EventsController> _logger;

    public EventsController(
        IEventRepository repository,
        IEventValidator validator,
        IEventProcessor processor,
        ILogger<EventsController> logger)
    {
        _repository = repository;
        _validator  = validator;
        _processor  = processor;
        _logger     = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] EventRequest request)
    {
        _logger.LogInformation(
            "Request received: EventType={EventType} Timestamp={Timestamp}",
            request.EventType, request.Timestamp);

        var errors = _validator.Validate(request);
        if (errors.Count > 0)
        {
            _logger.LogWarning("Validation failed: {@Errors}", errors);
            return BadRequest(errors);
        }

        var posEvent = PosEvent.From(request);

        await _repository.AddEventAsync(posEvent);

        _ = Task.Run(async () =>
        {
            try { await _processor.ProcessAsync(posEvent); }
            catch (Exception ex) { _logger.LogError(ex, "Processing failed for {EventId}", posEvent.Id); }
        });

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var events = await _repository.GetAllEventsAsync();
        return Ok(events);
    }
}
