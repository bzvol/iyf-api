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
        _context.RegularEvents.Find(id) ?? throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));

    public RegularEvent CreateEvent(CreateEventRequest value)
    {
        var eventEntry = _context.RegularEvents.Add(new RegularEvent
        {
            Title = value.Title,
            Details = value.Details,
        });
        _context.SaveChanges();
        return eventEntry.Entity;
    }

    public RegularEvent UpdateEvent(ulong id, UpdateEventRequest value)
    {
        var @event = _context.RegularEvents.Find(id);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));
        
        @event.Title = value.Title;
        @event.Details = value.Details;
        @event.Status = value.Status;
        
        var updatedEvent = _context.RegularEvents.Update(@event);
        _context.SaveChanges();
        
        return updatedEvent.Entity;
    }

    public RegularEvent? DeleteEvent(ulong id)
    {
        var @event = _context.RegularEvents.Find(id);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));
        
        if (@event.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");
        
        var deletedEvent = _context.RegularEvents.Remove(@event);
        _context.SaveChanges();
        return deletedEvent.Entity;
    }
}