﻿using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Types;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class EventRepository(ApplicationDbContext context) : IEventRepository
{
    public IEnumerable<Event> GetAllEvents() => context.Events;

    public Event GetEvent(ulong id) =>
        context.Events.Find(id) ?? throw new KeyNotFoundException(NoEventFoundMessage(id));

    public Event CreateEvent(CreateEventRequest value, string userId)
    {
        var eventEntry = context.Events.Add(new Event
        {
            Title = value.Title,
            Details = value.Details,
            StartTime = value.StartTime,
            EndTime = value.EndTime,
            Location = value.Location,
            CreatedBy = userId
        });
        context.SaveChanges();
        return eventEntry.Entity;
    }

    public Event UpdateEvent(ulong id, UpdateEventRequest value, string userId)
    {
        var @event = context.Events.Find(id);
        if (@event == null) throw new KeyNotFoundException(NoEventFoundMessage(id));

        @event.Title = value.Title;
        @event.Details = value.Details;
        @event.Status = value.Status;
        @event.StartTime = value.StartTime;
        @event.EndTime = value.EndTime;
        @event.Location = value.Location;
        @event.UpdatedBy = userId;

        var updatedEvent = context.Events.Update(@event);
        context.SaveChanges();

        return updatedEvent.Entity;
    }

    public Event? DeleteEvent(ulong id)
    {
        var @event = context.Events.Find(id);
        if (@event == null) throw new KeyNotFoundException(NoEventFoundMessage(id));

        if (@event.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");

        var deletedEvent = context.Events.Remove(@event);
        context.SaveChanges();
        return deletedEvent.Entity;
    }

    public static string NoEventFoundMessage(ulong? id) =>
        "The specified event " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}