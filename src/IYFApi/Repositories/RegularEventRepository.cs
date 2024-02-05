using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Types;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class RegularEventRepository(ApplicationDbContext context) : IRegularEventRepository
{
    public IEnumerable<RegularEvent> GetAllEvents() => context.RegularEvents;

    public RegularEvent GetEvent(ulong id) =>
        context.RegularEvents.Find(id) ?? throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));

    public RegularEvent CreateEvent(CreateEventRequest value)
    {
        var eventEntry = context.RegularEvents.Add(new RegularEvent
        {
            Title = value.Title,
            Details = value.Details,
        });
        context.SaveChanges();
        return eventEntry.Entity;
    }

    public RegularEvent UpdateEvent(ulong id, UpdateEventRequest value)
    {
        var @event = context.RegularEvents.Find(id);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));

        @event.Title = value.Title;
        @event.Details = value.Details;
        @event.Status = value.Status;

        var updatedEvent = context.RegularEvents.Update(@event);
        context.SaveChanges();

        return updatedEvent.Entity;
    }

    public RegularEvent? DeleteEvent(ulong id)
    {
        var @event = context.RegularEvents.Find(id);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));

        if (@event.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");

        var deletedEvent = context.RegularEvents.Remove(@event);
        context.SaveChanges();
        return deletedEvent.Entity;
    }
}