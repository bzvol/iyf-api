using IYFApi.Models;
using IYFApi.Models.Request;

namespace IYFApi.Repositories.Interfaces;

public interface IEventRepository
{
    public IEnumerable<Event> GetAllEvents();
    public Event GetEvent(ulong id);
    public Event CreateEvent(CreateEventRequest value, string userId);
    public Event UpdateEvent(ulong id, UpdateEventRequest value, string userId);
    public Event? DeleteEvent(ulong id);
}