using IYFApi.Models;
using IYFApi.Models.Request;

namespace IYFApi.Repositories.Interfaces;

public interface IGuestRepository
{
    public IEnumerable<EventGuest> GetGuestsForEvent(ulong eventId);
    public EventGuest CreateGuest(ulong eventId, CreateGuestRequest value);
    public EventGuest UpdateGuest(ulong guestId, UpdateGuestRequest value);
    public EventGuest? DeleteGuest(ulong guestId);
}