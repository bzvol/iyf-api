using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class RegularEventRepository(ApplicationDbContext context) : IRegularEventRepository
{
    public IEnumerable<RegularEventResponse> GetAllEvents() => context.RegularEvents
        .Where(@event => @event.Status == Status.Published)
        .ToList().Select(ConvertToEventResponse);

    public IEnumerable<RegularEventAuthorizedResponse> GetAllEventsAuthorized() => context.RegularEvents
        .ToList().Select(ConvertToEventAuthorizedResponse);
    
    public RegularEventResponse GetEvent(ulong id)
    {
        var @event = context.RegularEvents.Find(id) ?? 
                     throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));
        if (@event.Status != Status.Published) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));
        return ConvertToEventResponse(@event);
    }
    
    public RegularEventAuthorizedResponse GetEventAuthorized(ulong id)
    {
        var @event = context.RegularEvents.Find(id) ??
                     throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));
        return ConvertToEventAuthorizedResponse(@event);
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
        return ConvertToEventAuthorizedResponse(eventEntry.Entity);
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

        return ConvertToEventAuthorizedResponse(updatedEvent.Entity);
    }

    public RegularEventResponse DeleteEvent(ulong id)
    {
        var @event = context.RegularEvents.Find(id);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));

        if (@event.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");

        var deletedEvent = context.RegularEvents.Remove(@event);
        context.SaveChanges();
        return ConvertToEventAuthorizedResponse(deletedEvent.Entity);
    }

    private static RegularEventResponse ConvertToEventResponse(RegularEvent @event) => new()
    {
        Id = @event.Id,
        Title = @event.Title,
        Details = @event.Details,
        Schedule = new RegularEventSchedule
        {
            Time = @event.Time,
            Location = @event.Location
        }
    };
    
    private static RegularEventAuthorizedResponse ConvertToEventAuthorizedResponse(RegularEvent @event) => new()
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