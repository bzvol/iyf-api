﻿using IYFApi.Filters;
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
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public IActionResult CreateEvent([FromBody] CreateEventRequest value)
    {
        var @event = _repository.CreateEvent(value);
        return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
    }

    [HttpPut("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public RegularEvent UpdateEvent(ulong id, [FromBody] UpdateEventRequest value) =>
        _repository.UpdateEvent(id, value);

    [HttpDelete("{id}")]
    [AdminAuthorizationFilter(AdminRole.ContentManager)]
    public RegularEvent? DeleteEvent(ulong id) => _repository.DeleteEvent(id);
}