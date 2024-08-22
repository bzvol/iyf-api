using IYFApi.Models.Response;

namespace IYFApi.Models.Request;

public class CreateRegularEventRequest
{
    public string Title { get; init; } = null!;
    public string Details { get; init; } = null!;
    public RegularEventSchedule Schedule { get; init; } = null!;
}
