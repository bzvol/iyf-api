using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _repository;
    private readonly IVisitorRepository _visitorRepository;

    public EventsController(IEventRepository repository, IVisitorRepository visitorRepository)
    {
        _repository = repository;
        _visitorRepository = visitorRepository;
    }

    [HttpGet]
    public IEnumerable<Event> GetAllEvents() => _repository.GetAllEvents();

    [HttpGet("{id}")]
    public Event GetEvent(ulong id) => _repository.GetEvent(id);

    [HttpPost]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public IActionResult CreateEvent([FromBody] CreateEventRequest value)
    {
        var @event = _repository.CreateEvent(value);
        return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
    }

    [HttpPut("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public Event UpdateEvent(ulong id, [FromBody] UpdateEventRequest value) => _repository.UpdateEvent(id, value);

    [HttpDelete("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public Event? DeleteEvent(ulong id) => _repository.DeleteEvent(id);

    [HttpGet("{id}/visitors")]
    [AdminAuthorizationFilter(AdminRole.GuestManager)]
    public IEnumerable<EventVisitor> GetVisitorsForEvent(ulong id) => _visitorRepository.GetVisitorsForEvent(id);

    [HttpPost("{id}/visitors")]
    // Creating a guest will be done by the guest itself,
    // with the form on an event's page.
    public IActionResult CreateVisitor(ulong id, [FromBody] CreateVisitorRequest value)
    {
        var visitor = _visitorRepository.CreateVisitor(id, value);
        return StatusCode(201, visitor);
    }

    [HttpPut("{id}/visitors/{visitorId}")]
    [AdminAuthorizationFilter(AdminRole.GuestManager)]
    public EventVisitor UpdateVisitor(ulong visitorId, [FromBody] UpdateVisitorRequest value) =>
        _visitorRepository.UpdateVisitor(visitorId, value);

    [HttpDelete("{id}/visitors/{visitorId}")]
    [AdminAuthorizationFilter(AdminRole.GuestManager)]
    public EventVisitor? DeleteVisitor(ulong visitorId) => _visitorRepository.DeleteVisitor(visitorId);
}