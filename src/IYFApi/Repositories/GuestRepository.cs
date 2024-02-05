using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class GuestRepository(ApplicationDbContext context) : IGuestRepository
{
    public IEnumerable<EventGuest> GetGuestsForEvent(ulong eventId)
    {
        var @event = context.Events.Find(eventId);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(eventId));

        return from guest in context.EventGuests
            where guest.EventId == eventId
            select guest;
    }

    public EventGuest CreateGuest(ulong eventId, CreateGuestRequest value)
    {
        var @event = context.Events.Find(eventId);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(eventId));

        var guest = context.EventGuests.Add(new EventGuest
        {
            EventId = eventId,
            Name = value.Name,
            Email = value.Email,
            Phone = value.Phone,
            Age = value.Age,
            City = value.City,
            Source = value.Source,
        });
        context.SaveChanges();
        return guest.Entity;
    }

    public EventGuest UpdateGuest(ulong guestId, UpdateGuestRequest value)
    {
        var guest = context.EventGuests.Find(guestId);
        if (guest == null) throw new KeyNotFoundException(NoGuestFoundMessage(guestId));

        guest.Name = value.Name;
        guest.Email = value.Email;
        guest.Phone = value.Phone;
        guest.Age = value.Age;
        guest.City = value.City;
        guest.Source = value.Source;

        var updatedGuest = context.EventGuests.Update(guest);
        context.SaveChanges();

        return updatedGuest.Entity;
    }

    public EventGuest? DeleteGuest(ulong guestId)
    {
        var guest = context.EventGuests.Find(guestId);
        if (guest == null) throw new KeyNotFoundException(NoGuestFoundMessage(guestId));

        var deletedGuest = context.EventGuests.Remove(guest);
        context.SaveChanges();

        return deletedGuest.Entity;
    }

    private static string NoGuestFoundMessage(ulong? id) =>
        "The specified guest " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}