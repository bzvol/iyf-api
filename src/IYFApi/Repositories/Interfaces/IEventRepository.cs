using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;

namespace IYFApi.Repositories.Interfaces;

public interface IEventRepository
{
    public IEnumerable<EventResponse> GetAllEvents();
    public Task<IEnumerable<EventAuthorizedResponse>> GetAllEventsAuthorized();
    public EventResponse GetEvent(ulong id);
    public Task<EventAuthorizedResponse> GetEventAuthorized(ulong id);
    public Task<EventResponse> CreateEvent(CreateEventRequest value, string userId);
    public Task<EventResponse> UpdateEvent(ulong id, UpdateEventRequest value, string userId);
    public Task<EventResponse> DeleteEvent(ulong id);
}