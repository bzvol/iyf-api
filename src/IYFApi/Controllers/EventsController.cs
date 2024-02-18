﻿using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class EventsController(IEventRepository repository, IGuestRepository guestRepository)
    : ControllerBase
{
    [HttpGet]
    public IEnumerable<Event> GetAllEvents() => repository.GetAllEvents();

    [HttpGet("{id}")]
    public Event GetEvent(ulong id) => repository.GetEvent(id);

    [HttpPost]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public IActionResult CreateEvent([FromBody] CreateEventRequest value)
    {
        var @event = repository.CreateEvent(value, this.GetUid());
        return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
    }

    [HttpPut("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public Event UpdateEvent(ulong id, [FromBody] UpdateEventRequest value) =>
        repository.UpdateEvent(id, value, this.GetUid());

    [HttpDelete("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public Event? DeleteEvent(ulong id) => repository.DeleteEvent(id);

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