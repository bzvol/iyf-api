namespace IYFApi.Models.Response;

public class EventResponse
{
    public ulong Id { get; init; }
    public string Title { get; init; } = null!;
    public string Details { get; init; } = null!;
    public EventSchedule Schedule { get; init; } = null!;
}

public class EventAuthorizedResponse : EventResponse
{
    public Status Status { get; init; }
    public ObjectMetadata Metadata { get; init; } = null!;
}

public class EventSchedule
{
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public string? Location { get; init; } = null!;
}
