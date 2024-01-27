using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Types;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;
    
    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Event> GetAllEvents() => _context.Events;

    public Event GetEvent(ulong id) => 
        _context.Events.Find(id) ?? throw new KeyNotFoundException(NoEventFoundMessage(id));

    public Event CreateEvent(CreateEventRequest value)
    {
        var eventEntry = _context.Events.Add(new Event
        {
            Title = value.Title,
            Description = value.Description,
        });
        _context.SaveChanges();
        return eventEntry.Entity;
    }

    public Event UpdateEvent(ulong id, UpdateEventRequest value)
    {
        var @event = _context.Events.Find(id);
        if (@event == null) throw new KeyNotFoundException(NoEventFoundMessage(id));
        
        @event.Title = value.Title;
        @event.Description = value.Description;
        @event.Status = value.Status;
        
        var updatedEvent = _context.Events.Update(@event);
        _context.SaveChanges();
        
        return updatedEvent.Entity;
    }

    public Event? DeleteEvent(ulong id)
    {
        var @event = _context.Events.Find(id);
        if (@event == null) throw new KeyNotFoundException(NoEventFoundMessage(id));
        
        if (@event.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");
        
        var deletedEvent = _context.Events.Remove(@event);
        _context.SaveChanges();
        return deletedEvent.Entity;
    }
    
    public static string NoEventFoundMessage(ulong? id) =>
        "The specified event " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}