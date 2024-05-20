using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;
using IYFApi.Repositories.Interfaces;

namespace IYFApi.Repositories;

public class EventRepository(ApplicationDbContext context) : IEventRepository
{
    public IEnumerable<EventResponse> GetAllEvents() => context.Events
        .Where(@event => @event.Status == Status.Published)
        .ToList().Select(ConvertToEventResponse);
    
    public IEnumerable<EventAuthorizedResponse> GetAllEventsAuthorized() => context.Events
        .ToList().Select(ConvertToEventAuthorizedResponse);

    public EventResponse GetEvent(ulong id)
    {
        var @event = context.Events.Find(id) ?? throw new KeyNotFoundException(NoEventFoundMessage(id));
        if (@event.Status != Status.Published) throw new KeyNotFoundException(NoEventFoundMessage(id));
        return ConvertToEventResponse(@event);
    }
    
    public EventAuthorizedResponse GetEventAuthorized(ulong id)
    {
        var @event = context.Events.Find(id) ?? throw new KeyNotFoundException(NoEventFoundMessage(id));
        return ConvertToEventAuthorizedResponse(@event);
    }

    public EventResponse CreateEvent(CreateEventRequest value, string userId)
    {
        var eventEntry = context.Events.Add(new Event
        {
            Title = value.Title,
            Details = value.Details,
            StartTime = value.StartTime,
            EndTime = value.EndTime,
            Location = value.Location,
            CreatedBy = userId
        });
        context.SaveChanges();
        return ConvertToEventAuthorizedResponse(eventEntry.Entity);
    }

    public EventResponse UpdateEvent(ulong id, UpdateEventRequest value, string userId)
    {
        var @event = context.Events.Find(id);
        if (@event == null) throw new KeyNotFoundException(NoEventFoundMessage(id));

        @event.Title = value.Title;
        @event.Details = value.Details;
        @event.Status = value.Status;
        @event.StartTime = value.StartTime;
        @event.EndTime = value.EndTime;
        @event.Location = value.Location;
        @event.UpdatedBy = userId;
        
        if (@event.Status != Status.Published && value.Status == Status.Published)
            @event.PublishedAt = DateTime.UtcNow;
        else if (@event.Status == Status.Published && value.Status != Status.Published)
            @event.PublishedAt = null;

        var updatedEvent = context.Events.Update(@event);
        context.SaveChanges();

        return ConvertToEventAuthorizedResponse(updatedEvent.Entity);
    }

    public EventResponse DeleteEvent(ulong id)
    {
        var @event = context.Events.Find(id);
        if (@event == null) throw new KeyNotFoundException(NoEventFoundMessage(id));

        if (@event.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");

        var deletedEvent = context.Events.Remove(@event);
        context.SaveChanges();
        return ConvertToEventAuthorizedResponse(deletedEvent.Entity);
    }

    private static EventResponse ConvertToEventResponse(Event @event) => new()
    {
        Id = @event.Id,
        Title = @event.Title,
        Details = @event.Details,
        Schedule = new EventSchedule
        {
            StartTime = @event.StartTime,
            EndTime = @event.EndTime,
            Location = @event.Location
        },
        PublishedAt = @event.PublishedAt
    };
    
    private static EventAuthorizedResponse ConvertToEventAuthorizedResponse(Event @event) => new()
    {
        Id = @event.Id,
        Title = @event.Title,
        Details = @event.Details,
        Schedule = new EventSchedule
        {
            StartTime = @event.StartTime,
            EndTime = @event.EndTime,
            Location = @event.Location
        },
        PublishedAt = @event.PublishedAt,
        Status = @event.Status,
        Metadata = new ObjectMetadata
        {
            CreatedAt = @event.CreatedAt,
            CreatedBy = @event.CreatedBy,
            UpdatedAt = @event.UpdatedAt,
            UpdatedBy = @event.UpdatedBy
        }
    };

    public static string NoEventFoundMessage(ulong? id) =>
        "The specified event " + (id.HasValue ? $"({id}) " : "") + "could not be found.";
}