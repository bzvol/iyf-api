using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
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

    public GuestResponse CreateGuest(ulong eventId, CreateGuestRequest value)
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

        var customFields = new Dictionary<string, string>();
        foreach (var kv in value.Custom)
        {
            var customField = context.EventCustomFields.Add(new EventCustomField
            {
                EventId = eventId,
                GuestId = guest.Entity.Id,
                FieldName = kv.Key,
                FieldValue = kv.Value,
            });
            customFields.Add(customField.Entity.FieldName, customField.Entity.FieldValue);
        }
        
        context.SaveChanges();

        var response = new GuestResponse
        {
            Id = guest.Entity.Id,
            EventId = eventId,
            Name = guest.Entity.Name,
            Email = guest.Entity.Email,
            Phone = guest.Entity.Phone,
            Age = guest.Entity.Age,
            City = guest.Entity.City,
            Source = guest.Entity.Source,
            Custom = customFields,
            AddedAt = guest.Entity.AddedAt,
        };

        return response;
    }

    public GuestResponse UpdateGuest(ulong guestId, UpdateGuestRequest value)
    {
        var guest = context.EventGuests.Find(guestId);
        if (guest == null) throw new KeyNotFoundException(NoGuestFoundMessage(guestId));

        guest.Name = value.Name;
        guest.Email = value.Email;
        guest.Phone = value.Phone;
        guest.Age = value.Age;
        guest.City = value.City;
        guest.Source = value.Source;

        foreach (var kv in value.Custom)
        {
            var customField = context.EventCustomFields.FirstOrDefault(cf =>
                cf.EventId == guest.EventId && cf.GuestId == guest.Id && cf.FieldName == kv.Key);
            if (customField == null) throw new KeyNotFoundException(
                $"The specified custom field '{kv.Key}' could not be found for guest {guest.Id}.");
            
            customField.FieldValue = kv.Value;
            
            context.EventCustomFields.Update(customField);
        }

        var updatedGuest = context.EventGuests.Update(guest);
        context.SaveChanges();

        var response = new GuestResponse
        {
            Id = updatedGuest.Entity.Id,
            EventId = updatedGuest.Entity.EventId,
            Name = updatedGuest.Entity.Name,
            Email = updatedGuest.Entity.Email,
            Phone = updatedGuest.Entity.Phone,
            Age = updatedGuest.Entity.Age,
            City = updatedGuest.Entity.City,
            Source = updatedGuest.Entity.Source,
            Custom = new Dictionary<string, string>(from cf in context.EventCustomFields
                where cf.EventId == updatedGuest.Entity.EventId && cf.GuestId == updatedGuest.Entity.Id
                select new KeyValuePair<string, string>(cf.FieldName, cf.FieldValue)),
            AddedAt = updatedGuest.Entity.AddedAt,
        };

        return response;
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