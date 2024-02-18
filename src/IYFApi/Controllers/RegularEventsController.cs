using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/regular")]
public class RegularEventsController(IRegularEventRepository repository) : ControllerBase
{
    [HttpGet]
    public IEnumerable<RegularEvent> GetAllEvents() => repository.GetAllEvents();

    [HttpGet("{id}")]
    public RegularEvent GetEvent(ulong id) => repository.GetEvent(id);

    [HttpPost]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public IActionResult CreateEvent([FromBody] CreateEventRequest value)
    {
        var @event = repository.CreateEvent(value);
        return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
    }

    [HttpPut("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public RegularEvent UpdateEvent(ulong id, [FromBody] UpdateEventRequest value) =>
        repository.UpdateEvent(id, value);

    [HttpDelete("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public RegularEvent? DeleteEvent(ulong id) => repository.DeleteEvent(id);
}