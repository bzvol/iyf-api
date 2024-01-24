using IYFApi.Models;
using IYFApi.Models.Types;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class RegularEventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public RegularEventsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IEnumerable<RegularEvent> Get()
    {
        return _context.RegularEvents;
    }
    
    [HttpGet("{id}")]
    public RegularEvent? Get(ulong id)
    {
        return _context.RegularEvents.Find(id);
    }
    
    [HttpPost]
    public RegularEvent Post([FromBody] RegularEvent value)
    {
        var eventEntry = _context.RegularEvents.Add(value);
        _context.SaveChanges();
        return eventEntry.Entity;
    }
    
    [HttpDelete("{id}")]
    public RegularEvent? Delete(ulong id)
    {
        var eventEntity = _context.RegularEvents.Find(id);
        if (eventEntity == null) return null;
        
        if (eventEntity.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");
        
        var deletedEvent = _context.RegularEvents.Remove(eventEntity);
        _context.SaveChanges();
        return deletedEvent.Entity;
    }
}