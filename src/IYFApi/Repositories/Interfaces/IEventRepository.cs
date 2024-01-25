using IYFApi.Models;
using IYFApi.Models.Request;

namespace IYFApi.Repositories.Interfaces;

public interface IEventRepository
{
    public IEnumerable<Event> GetAllEvents();
    public Event GetEvent(ulong id);
    public Event CreateEvent(CreateEventRequest value);
    public Event UpdateEvent(ulong id, UpdateEventRequest value);
    public Event? DeleteEvent(ulong id);
}