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
    [OptionalAdminAuthorizationFilter]
    public async Task<IEnumerable<RegularEventResponse>> GetAllEvents() => (bool)HttpContext.Items["IsAuthorized"]!
        ? await repository.GetAllEventsAuthorized()
        : repository.GetAllEvents();

    [HttpGet("{id}")]
    [OptionalAdminAuthorizationFilter]
    public async Task<RegularEventResponse> GetEvent(ulong id) => (bool)HttpContext.Items["IsAuthorized"]!
        ? await repository.GetEventAuthorized(id)
        : repository.GetEvent(id);

    [HttpPost]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest value)
    {
        var @event = await repository.CreateEvent(value, ((UserRecordFix)HttpContext.Items["User"]!).Uid);
        return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
    }

    [HttpPut("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<RegularEventResponse> UpdateEvent(ulong id, [FromBody] UpdateEventRequest value) =>
        await repository.UpdateEvent(id, value, ((UserRecordFix)HttpContext.Items["User"]!).Uid);

    [HttpDelete("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<RegularEventResponse> DeleteEvent(ulong id) => await repository.DeleteEvent(id);
}