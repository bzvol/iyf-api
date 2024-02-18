using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Models.Types;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class RegularEventRepository(ApplicationDbContext context) : IRegularEventRepository
{
    public IEnumerable<RegularEventResponse> GetAllEvents() => context.RegularEvents.Select(ConvertToResponseObject);

    public RegularEventResponse GetEvent(ulong id)
    {
        var @event = context.RegularEvents.Find(id) ??
                     throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));
        return ConvertToResponseObject(@event);
    }

    public RegularEventResponse CreateEvent(CreateEventRequest value, string userId)
    {
        var eventEntry = context.RegularEvents.Add(new RegularEvent
        {
            Title = value.Title,
            Details = value.Details,
            Time = value.Time!,
            Location = value.Location,
            CreatedBy = userId
        });
        context.SaveChanges();
        return ConvertToResponseObject(eventEntry.Entity);
    }

    public RegularEventResponse UpdateEvent(ulong id, UpdateEventRequest value, string userId)
    {
        var @event = context.RegularEvents.Find(id);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));

        @event.Title = value.Title;
        @event.Details = value.Details;
        @event.Status = value.Status;
        @event.Time = value.Time;
        @event.Location = value.Location;
        @event.UpdatedBy = userId;

        var updatedEvent = context.RegularEvents.Update(@event);
        context.SaveChanges();

        return ConvertToResponseObject(updatedEvent.Entity);
    }

    public RegularEventResponse DeleteEvent(ulong id)
    {
        var @event = context.RegularEvents.Find(id);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));

        if (@event.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");

        var deletedEvent = context.RegularEvents.Remove(@event);
        context.SaveChanges();
        return ConvertToResponseObject(deletedEvent.Entity);
    }

    private static RegularEventResponse ConvertToResponseObject(RegularEvent @event) =>
        new RegularEventResponse
        {
            Id = @event.Id,
            Title = @event.Title,
            Details = @event.Details,
            Schedule = new RegularEventSchedule
            {
                Time = @event.Time,
                Location = @event.Location
            },

            Status = @event.Status,
            Metadata = new ObjectMetadata
            {
                CreatedAt = @event.CreatedAt,
                CreatedBy = @event.CreatedBy,
                UpdatedAt = @event.UpdatedAt,
                UpdatedBy = @event.UpdatedBy
            }
        };
}