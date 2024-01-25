using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Types;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class RegularEventRepository : IRegularEventRepository
{
    private readonly ApplicationDbContext _context;
    
    public RegularEventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<RegularEvent> GetAllEvents() => _context.RegularEvents;

    public RegularEvent GetEvent(ulong id) => 
        _context.RegularEvents.Find(id) ?? throw new KeyNotFoundException(NoEventFoundMessage(id));

    public RegularEvent CreateEvent(CreateEventRequest value)
    {
        var eventEntry = _context.RegularEvents.Add(new RegularEvent
        {
            Title = value.Title,
            Description = value.Description,
        });
        _context.SaveChanges();
        return eventEntry.Entity;
    }

    public RegularEvent UpdateEvent(ulong id, UpdateEventRequest value)
    {
        var eventEntity = _context.RegularEvents.Find(id);
        if (eventEntity == null) throw new KeyNotFoundException(NoEventFoundMessage(id));
        
        eventEntity.Title = value.Title;
        eventEntity.Description = value.Description;
        eventEntity.Status = value.Status;
        
        var updatedEvent = _context.RegularEvents.Update(eventEntity);
        _context.SaveChanges();
        
        return updatedEvent.Entity;
    }

    public RegularEvent? DeleteEvent(ulong id)
    {
        var eventEntity = _context.RegularEvents.Find(id);
        if (eventEntity == null) throw new KeyNotFoundException(NoEventFoundMessage(id));
        
        if (eventEntity.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");
        
        var deletedEvent = _context.RegularEvents.Remove(eventEntity);
        _context.SaveChanges();
        return deletedEvent.Entity;
    }
    
    private static string NoEventFoundMessage(ulong? id) =>
        "The specified event " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}