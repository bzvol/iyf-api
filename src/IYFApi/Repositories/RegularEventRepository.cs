using FirebaseAdmin.Auth;
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

    public async Task<IEnumerable<RegularEventAuthorizedResponse>> GetAllEventsAuthorized() => await Task.WhenAll(
        context.RegularEvents.ToList().Select(ConvertToEventAuthorizedResponse));
    
    public RegularEventResponse GetEvent(ulong id)
    {
        var @event = context.RegularEvents.Find(id) ?? 
                     throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));
        if (@event.Status != Status.Published) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));
        return ConvertToEventResponse(@event);
    }
    
    public async Task<RegularEventAuthorizedResponse> GetEventAuthorized(ulong id)
    {
        var @event = await context.RegularEvents.FindAsync(id) ??
                     throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));
        return await ConvertToEventAuthorizedResponse(@event);
    }

    public async Task<RegularEventResponse> CreateEvent(CreateRegularEventRequest value, string userId)
    {
        var eventEntry = context.RegularEvents.Add(new RegularEvent
        {
            Title = value.Title,
            Details = value.Details,
            Time = value.Schedule.Time,
            Location = value.Schedule.Location,
            CreatedBy = userId,
            UpdatedBy = userId
        });
        await context.SaveChangesAsync();
        return await ConvertToEventAuthorizedResponse(eventEntry.Entity);
    }

    public async Task<RegularEventResponse> UpdateEvent(ulong id, UpdateRegularEventRequest value, string userId)
    {
        var @event = await context.RegularEvents.FindAsync(id);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));

        @event.Title = value.Title;
        @event.Details = value.Details;
        @event.Status = value.Status;
        @event.Time = value.Schedule.Time;
        @event.Location = value.Schedule.Location;
        @event.UpdatedBy = userId;

        var updatedEvent = context.RegularEvents.Update(@event);
        await context.SaveChangesAsync();

        return await ConvertToEventAuthorizedResponse(updatedEvent.Entity);
    }

    public async Task<RegularEventResponse> DeleteEvent(ulong id)
    {
        var @event = await context.RegularEvents.FindAsync(id);
        if (@event == null) throw new KeyNotFoundException(EventRepository.NoEventFoundMessage(id));

        if (@event.Status != Status.Draft)
            throw new InvalidOperationException("You may only delete draft events.");

        var deletedEvent = context.RegularEvents.Remove(@event);
        await context.SaveChangesAsync();
        return await ConvertToEventAuthorizedResponse(deletedEvent.Entity);
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
    
    private static async Task<RegularEventAuthorizedResponse> ConvertToEventAuthorizedResponse(RegularEvent @event) => new()
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
            CreatedBy = await FirebaseAuth.DefaultInstance.GetUserAsync(@event.CreatedBy),
            UpdatedAt = @event.UpdatedAt,
            UpdatedBy = await FirebaseAuth.DefaultInstance.GetUserAsync(@event.UpdatedBy)
        }
    };
}