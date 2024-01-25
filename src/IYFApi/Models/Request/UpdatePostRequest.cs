using IYFApi.Models.Types;

namespace IYFApi.Models.Request;

public class UpdatePostRequest
{
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public Status Status { get; set; }
}