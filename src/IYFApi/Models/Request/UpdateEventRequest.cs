using IYFApi.Models.Types;

namespace IYFApi.Models.Request;

public class UpdateEventRequest
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Status Status { get; set; }
}