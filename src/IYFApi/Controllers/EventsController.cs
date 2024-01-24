using IYFApi.Models;
using IYFApi.Models.Types;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IEnumerable<Event> Get()
    {
        return _context.Events;
    }
    
    [HttpGet("{id}")]
    public Event? Get(ulong id)
    {
        return _context.Events.Find(id);
    }
    
    [HttpPost]
    public Event Post([FromBody] Event value)
    {
        var eventEntry = _context.Events.Add(value);
        _context.SaveChanges();
        return eventEntry.Entity;
    }
    
    [HttpDelete("{id}")]
    public Event? Delete(ulong id)
    {
        var eventEntity = _context.Events.Find(id);
        if (eventEntity == null) return null;
        
        if (eventEntity.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");
        
        var deletedEvent = _context.Events.Remove(eventEntity);
        _context.SaveChanges();
        return deletedEvent.Entity;
    }
}