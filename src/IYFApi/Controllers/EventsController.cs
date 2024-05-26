using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Repositories.Interfaces;
using IYFApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class EventsController(IEventRepository repository, IGuestRepository guestRepository)
    : ControllerBase
{
    [HttpGet]
    [OptionalAdminAuthorizationFilter]
    public async Task<IEnumerable<EventResponse>> GetAllEvents() => (bool)HttpContext.Items["IsAuthorized"]!
        ? await repository.GetAllEventsAuthorized()
        : repository.GetAllEvents();

    [HttpGet("{id}")]
    [OptionalAdminAuthorizationFilter]
    public async Task<EventResponse> GetEvent(ulong id) => (bool)HttpContext.Items["IsAuthorized"]!
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
    public async Task<EventResponse> UpdateEvent(ulong id, [FromBody] UpdateEventRequest value) =>
        await repository.UpdateEvent(id, value, ((UserRecordFix)HttpContext.Items["User"]!).Uid);

    [HttpDelete("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public async Task<EventResponse> DeleteEvent(ulong id) => await repository.DeleteEvent(id);

    [HttpGet("{id}/guests")]
    [AdminAuthorizationFilter(AdminRole.GuestManager)]
    public IEnumerable<EventGuest> GetGuestsForEvent(ulong id) => guestRepository.GetGuestsForEvent(id);

    [HttpPost("{id}/guests")]
    // Creating a guest will be done by the guest itself,
    // with the form on an event's page.
    public IActionResult CreateGuest(ulong id, [FromBody] CreateGuestRequest value)
    {
        var guest = guestRepository.CreateGuest(id, value);
        return StatusCode(201, guest);
    }

    [HttpPut("{id}/guests/{guestId}")]
    [AdminAuthorizationFilter(AdminRole.GuestManager)]
    public EventGuest UpdateGuest(ulong guestId, [FromBody] UpdateGuestRequest value) =>
        guestRepository.UpdateGuest(guestId, value);

    [HttpDelete("{id}/guests/{guestId}")]
    [AdminAuthorizationFilter(AdminRole.GuestManager)]
    public EventGuest? DeleteGuest(ulong guestId) => guestRepository.DeleteGuest(guestId);
}