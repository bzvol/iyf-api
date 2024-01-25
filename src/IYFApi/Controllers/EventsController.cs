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
    public Event CreateEvent([FromBody] CreateEventRequest value)
    {
        var createdEvent = _repository.CreateEvent(value);
        HttpContext.Response.StatusCode = 201;
        return createdEvent;
    }

    [HttpPut("{id}")]
    public Event UpdateEvent(ulong id, [FromBody] UpdateEventRequest value) => _repository.UpdateEvent(id, value);

    [HttpDelete("{id}")]
    public Event? DeleteEvent(ulong id) => _repository.DeleteEvent(id);

    [HttpGet("{id}/visitors")]
    public IEnumerable<EventVisitor> GetVisitorsForEvent(ulong id) => _visitorRepository.GetVisitorsForEvent(id);
    
    [HttpPost("{id}/visitors")]
    public EventVisitor CreateVisitor(ulong id, [FromBody] CreateVisitorRequest value) => _visitorRepository.CreateVisitor(id, value);
    
    [HttpPut("{id}/visitors/{visitorId}")]
    public EventVisitor UpdateVisitor(ulong visitorId, [FromBody] UpdateVisitorRequest value) => _visitorRepository.UpdateVisitor(visitorId, value);
    
    [HttpDelete("{id}/visitors/{visitorId}")]
    public EventVisitor? DeleteVisitor(ulong visitorId) => _visitorRepository.DeleteVisitor(visitorId);
}