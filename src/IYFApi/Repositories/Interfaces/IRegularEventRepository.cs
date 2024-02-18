using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;

namespace IYFApi.Repositories.Interfaces;

public interface IRegularEventRepository
{
    public IEnumerable<RegularEventResponse> GetAllEvents();
    public RegularEventResponse GetEvent(ulong id);
    public RegularEventResponse CreateEvent(CreateEventRequest value, string userId);
    public RegularEventResponse UpdateEvent(ulong id, UpdateEventRequest value, string userId);
    public RegularEventResponse DeleteEvent(ulong id);
}