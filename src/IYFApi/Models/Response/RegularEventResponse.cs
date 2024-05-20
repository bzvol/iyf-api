namespace IYFApi.Models.Response;

public class RegularEventResponse
{
    public ulong Id { get; init; }
    public string Title { get; init; } = null!;
    public string Details { get; init; } = null!;
    public RegularEventSchedule Schedule { get; init; } = null!;
}

public class RegularEventAuthorizedResponse : RegularEventResponse
{
    public Status Status { get; init; }
    public ObjectMetadata Metadata { get; init; } = null!;
}

public class RegularEventSchedule
{
    public string? Time { get; init; } = null!;
    public string? Location { get; init; } = null!;
}
