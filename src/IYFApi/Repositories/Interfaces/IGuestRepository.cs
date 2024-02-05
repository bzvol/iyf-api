using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;

namespace IYFApi.Repositories.Interfaces;

public interface IGuestRepository
{
    public IEnumerable<EventGuest> GetGuestsForEvent(ulong eventId);
    public GuestResponse CreateGuest(ulong eventId, CreateGuestRequest value);
    public GuestResponse UpdateGuest(ulong guestId, UpdateGuestRequest value);
    public EventGuest? DeleteGuest(ulong guestId);
}