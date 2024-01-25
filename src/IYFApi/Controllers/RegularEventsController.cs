using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/regular")]
public class RegularEventsController : ControllerBase
{
    private readonly IRegularEventRepository _repository;
    
    public RegularEventsController(IRegularEventRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IEnumerable<RegularEvent> GetAllEvents() => _repository.GetAllEvents();

    [HttpGet("{id}")]
    public RegularEvent GetEvent(ulong id) => _repository.GetEvent(id);

    [HttpPost]
    public RegularEvent CreateEvent([FromBody] CreateEventRequest value)
    {
        var createdEvent = _repository.CreateEvent(value);
        HttpContext.Response.StatusCode = 201;
        return createdEvent;
    }
    
    [HttpPut("{id}")]
    public RegularEvent UpdateEvent(ulong id, [FromBody] UpdateEventRequest value) => _repository.UpdateEvent(id, value);
    
    [HttpDelete("{id}")]
    public RegularEvent? DeleteEvent(ulong id) => _repository.DeleteEvent(id);
}