using IYFApi.Models;
using IYFApi.Models.Response;
using IYFApi.Services.Interfaces;

namespace IYFApi.Services;

public class InfoService(ApplicationDbContext context) : IInfoService
{
    public CountInfoResponse GetCountInfo()
    {
        var posts = context.Posts.GroupBy(post => post.Status)
            .Select(postGroup => new CountGroup { Status = postGroup.Key, Count = postGroup.Count() });

        var postCountInfo = new PostCountInfo
        {
            Total = posts.Sum(p => p.Count),
            Draft = GetCountForStatus(posts, Status.Draft),
            Published = GetCountForStatus(posts, Status.Published),
            Archived = GetCountForStatus(posts, Status.Archived)
        };

        var events = context.Events.GroupBy(ev => ev.Status)
            .Select(eventGroup => new CountGroup { Status = eventGroup.Key, Count = eventGroup.Count() });
        var upcomingEvents = context.Events.Count(ev => ev.StartTime > DateTime.Now);
        var pastEvents = context.Events.Count(ev => ev.EndTime < DateTime.Now);

        var guests = context.EventGuests.Count();
        var uniqueGuests = context.EventGuests.ToList().Distinct(new EventGuest.EquivalencyComparer()).Count();

        var eventCountInfo = new EventCountInfo
        {
            Total = events.Sum(e => e.Count),
            Upcoming = upcomingEvents,
            Past = pastEvents,
            Draft = GetCountForStatus(events, Status.Draft),
            Published = GetCountForStatus(events, Status.Published),
            Archived = GetCountForStatus(events, Status.Archived),
            TotalGuests = guests,
            UniqueGuests = uniqueGuests
        };

        var regularEvents = context.RegularEvents.GroupBy(re => re.Status)
            .Select(revGroup => new CountGroup { Status = revGroup.Key, Count = revGroup.Count() });

        var regularEventCountInfo = new RegularEventCountInfo
        {
            Total = regularEvents.Sum(re => re.Count),
            Draft = GetCountForStatus(regularEvents, Status.Draft),
            Published = GetCountForStatus(regularEvents, Status.Published),
            Archived = GetCountForStatus(regularEvents, Status.Archived)
        };

        return new CountInfoResponse
        {
            Posts = postCountInfo,
            Events = eventCountInfo,
            RegularEvents = regularEventCountInfo
        };
    }

    private class CountGroup
    {
        public Status Status { get; init; }
        public int Count { get; init; }
    }

    private static int GetCountForStatus(IEnumerable<CountGroup> groups, Status status) =>
        groups.FirstOrDefault(g => g.Status == status)?.Count ?? 0;
}