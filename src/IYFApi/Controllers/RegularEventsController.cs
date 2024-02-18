using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Repositories.Interfaces;
using IYFApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/regular")]
public class RegularEventsController(IRegularEventRepository repository) : ControllerBase
{
    [HttpGet]
    public IEnumerable<RegularEventResponse> GetAllEvents() => repository.GetAllEvents();

    [HttpGet("{id}")]
    public RegularEventResponse GetEvent(ulong id) => repository.GetEvent(id);

    [HttpPost]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest value)
    {
        var @event = repository.CreateEvent(value, await AuthService.GetUidFromRequest(Request));
        return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
    }

    [HttpPut("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<RegularEventResponse> UpdateEvent(ulong id, [FromBody] UpdateEventRequest value) =>
        repository.UpdateEvent(id, value, await AuthService.GetUidFromRequest(Request));

    [HttpDelete("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public RegularEventResponse DeleteEvent(ulong id) => repository.DeleteEvent(id);
}