using IYFApi.Models;
using IYFApi.Models.Request;

namespace IYFApi.Repositories.Interfaces;

public interface IVisitorRepository
{
    public IEnumerable<EventVisitor> GetVisitorsForEvent(ulong eventId);
    public EventVisitor CreateVisitor(ulong eventId, CreateVisitorRequest value);
    public EventVisitor UpdateVisitor(ulong visitorId, UpdateVisitorRequest value);
    public EventVisitor? DeleteVisitor(ulong visitorId);
}