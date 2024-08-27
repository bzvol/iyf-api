using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Repositories.Interfaces;

public interface IGuestRepository
{
    public IEnumerable<GuestResponse> GetGuestsForEvent(ulong eventId);
    public GuestResponse CreateGuest(ulong eventId, CreateGuestRequest value);
    public GuestResponse UpdateGuest(ulong guestId, UpdateGuestRequest value);
    public EventGuest? DeleteGuest(ulong guestId);
    public (MemoryStream, string, string) ExportGuests(ulong eventId);
}