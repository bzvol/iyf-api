using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class VisitorRepository : IVisitorRepository
{
    private readonly ApplicationDbContext _context;
    
    public VisitorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<EventVisitor> GetVisitorsForEvent(ulong eventId)
    {
        var eventEntity = _context.Events.Find(eventId);
        if (eventEntity == null) throw new KeyNotFoundException(NoEventFoundMessage(eventId));

        return from visitor in _context.EventVisitors
            where visitor.EventId == eventId
            select visitor;
    }

    public EventVisitor CreateVisitor(ulong eventId, CreateVisitorRequest value)
    {
        var eventEntity = _context.Events.Find(eventId);
        if (eventEntity == null) throw new KeyNotFoundException(NoEventFoundMessage(eventId));

        var visitor = _context.EventVisitors.Add(new EventVisitor
        {
            EventId = eventId,
            Name = value.Name,
            Email = value.Email,
            Phone = value.Phone,
            Age = value.Age,
            City = value.City,
            Source = value.Source,
        });
        _context.SaveChanges();
        return visitor.Entity;
    }

    public EventVisitor UpdateVisitor(ulong visitorId, UpdateVisitorRequest value)
    {
        var visitor = _context.EventVisitors.Find(visitorId);
        if (visitor == null) throw new KeyNotFoundException(NoVisitorFoundMessage(visitorId));

        visitor.Name = value.Name;
        visitor.Email = value.Email;
        visitor.Phone = value.Phone;
        visitor.Age = value.Age;
        visitor.City = value.City;
        visitor.Source = value.Source;
        
        var updatedVisitor = _context.EventVisitors.Update(visitor);
        _context.SaveChanges();
        
        return updatedVisitor.Entity;
    }

    public EventVisitor? DeleteVisitor(ulong visitorId)
    {
        var visitor = _context.EventVisitors.Find(visitorId);
        if (visitor == null) throw new KeyNotFoundException(NoVisitorFoundMessage(visitorId));

        var deletedVisitor = _context.EventVisitors.Remove(visitor);
        _context.SaveChanges();
        
        return deletedVisitor.Entity;
    }
    
    private static string NoEventFoundMessage(ulong? id) =>
        "The specified event " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
    
    private static string NoVisitorFoundMessage(ulong? id) =>
        "The specified visitor " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}