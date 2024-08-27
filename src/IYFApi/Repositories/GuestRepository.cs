using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Repositories.Interfaces;
using OfficeOpenXml;

namespace IYFApi.Repositories;

public class GuestRepository(ApplicationDbContext context) : IGuestRepository
{
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    
    public IEnumerable<GuestResponse> GetGuestsForEvent(ulong eventId)
    {
        var @event = context.Events.Find(eventId);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(eventId));

        return from guest in context.EventGuests
            where guest.EventId == eventId
            select new GuestResponse
            {
                Id = guest.Id,
                EventId = guest.EventId,
                Name = guest.Name,
                Email = guest.Email,
                Phone = guest.Phone,
                Age = guest.Age,
                City = guest.City,
                Source = guest.Source,
                Custom = new Dictionary<string, string>((from cf in context.EventCustomFields
                    where cf.EventId == guest.EventId && cf.GuestId == guest.Id
                    select new KeyValuePair<string, string>(cf.FieldName, cf.FieldValue)).ToList()),
                CreatedAt = guest.CreatedAt
            };
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
            Source = value.Source
        });

        var customFields = new Dictionary<string, string>();

        if (value.Custom != null)
            foreach (var kv in value.Custom)
            {
                var customField = context.EventCustomFields.Add(new EventCustomField
                {
                    EventId = eventId,
                    GuestId = guest.Entity.Id,
                    FieldName = kv.Key,
                    FieldValue = kv.Value
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
            CreatedAt = guest.Entity.CreatedAt
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

        if (value.Custom != null)
            foreach (var kv in value.Custom)
            {
                var customField = context.EventCustomFields.FirstOrDefault(cf =>
                    cf.EventId == guest.EventId && cf.GuestId == guest.Id && cf.FieldName == kv.Key);
                if (customField == null)
                    throw new KeyNotFoundException(
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
            Custom = new Dictionary<string, string>((from cf in context.EventCustomFields
                where cf.EventId == updatedGuest.Entity.EventId && cf.GuestId == updatedGuest.Entity.Id
                select new KeyValuePair<string, string>(cf.FieldName, cf.FieldValue)).ToList()),
            CreatedAt = updatedGuest.Entity.CreatedAt
        };

        return response;
    }

    public EventGuest DeleteGuest(ulong guestId)
    {
        var guest = context.EventGuests.Find(guestId);
        if (guest == null) throw new KeyNotFoundException(NoGuestFoundMessage(guestId));

        var deletedGuest = context.EventGuests.Remove(guest);
        context.SaveChanges();

        return deletedGuest.Entity;
    }

    public (MemoryStream, string, string) ExportGuests(ulong eventId)
    {
        var guests = GetGuestsForEvent(eventId).ToList();
        
        if (guests.Count == 0)
            throw new InvalidOperationException("No guests found for the specified event.");
        
        var fields = typeof(EventGuest).GetProperties().Select(p => p.Name).ToList();
        fields.Remove("EventId");
        fields.Remove("CreatedAt");
        fields.Remove("Custom");
        var customFields = guests.SelectMany(g => g.Custom.Keys).Distinct().ToList();
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Guests");
        
        for (var i = 0; i < fields.Count; i++)
            worksheet.Cells[1, i + 1].Value = fields[i];
        for (var i = 0; i < customFields.Count; i++)
            worksheet.Cells[1, fields.Count + i + 1].Value = customFields[i];
        worksheet.Cells[1, 1, 1, fields.Count + customFields.Count].Style.Font.Bold = true;

        for (var i = 0; i < guests.Count; i++)
        {
            var guest = guests[i];
            for (var j = 0; j < fields.Count; j++)
                worksheet.Cells[i + 2, j + 1].Value = typeof(GuestResponse).GetProperty(fields[j])?.GetValue(guest) ?? "";
            for (var j = 0; j < customFields.Count; j++)
                worksheet.Cells[i + 2, fields.Count + j + 1].Value = guest.Custom.GetValueOrDefault(customFields[j], "");
        }
        
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        var @event = context.Events.Find(eventId)!;
        var fileName = $"{@event.Title} - Guests.xlsx";
        
        return (stream, fileName, ExcelContentType);
    }

    private static string NoGuestFoundMessage(ulong? id) =>
        "The specified guest " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}