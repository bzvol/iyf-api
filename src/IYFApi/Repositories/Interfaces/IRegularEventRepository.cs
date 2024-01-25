using IYFApi.Models;
using IYFApi.Models.Request;

namespace IYFApi.Repositories.Interfaces;

public interface IRegularEventRepository
{
    public IEnumerable<RegularEvent> GetAllEvents();
    public RegularEvent GetEvent(ulong id);
    public RegularEvent CreateEvent(CreateEventRequest value);
    public RegularEvent UpdateEvent(ulong id, UpdateEventRequest value);
    public RegularEvent? DeleteEvent(ulong id);
}