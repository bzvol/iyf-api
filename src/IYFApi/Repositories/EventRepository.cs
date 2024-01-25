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
        var eventEntity = _context.Events.Find(id);
        if (eventEntity == null) throw new KeyNotFoundException(NoEventFoundMessage(id));
        
        eventEntity.Title = value.Title;
        eventEntity.Description = value.Description;
        eventEntity.Status = value.Status;
        
        var updatedEvent = _context.Events.Update(eventEntity);
        _context.SaveChanges();
        
        return updatedEvent.Entity;
    }

    public Event? DeleteEvent(ulong id)
    {
        var eventEntity = _context.Events.Find(id);
        if (eventEntity == null) throw new KeyNotFoundException(NoEventFoundMessage(id));
        
        if (eventEntity.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");
        
        var deletedEvent = _context.Events.Remove(eventEntity);
        _context.SaveChanges();
        return deletedEvent.Entity;
    }
    
    private static string NoEventFoundMessage(ulong? id) =>
        "The specified event " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}