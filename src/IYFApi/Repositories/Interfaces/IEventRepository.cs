using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;

namespace IYFApi.Repositories.Interfaces;

public interface IEventRepository
{
    public IEnumerable<EventResponse> GetAllEvents();
    public IEnumerable<EventAuthorizedResponse> GetAllEventsAuthorized();
    public EventResponse GetEvent(ulong id);
    public EventAuthorizedResponse GetEventAuthorized(ulong id);
    public EventResponse CreateEvent(CreateEventRequest value, string userId);
    public EventResponse UpdateEvent(ulong id, UpdateEventRequest value, string userId);
    public EventResponse DeleteEvent(ulong id);
}