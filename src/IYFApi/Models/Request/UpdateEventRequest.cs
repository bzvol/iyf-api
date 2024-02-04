using IYFApi.Models.Types;

namespace IYFApi.Models.Request;

public class UpdateEventRequest
{
    public string Title { get; set; } = null!;
    public string Details { get; set; } = null!;
    public Status Status { get; set; }
}