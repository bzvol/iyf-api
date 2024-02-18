using IYFApi.Models.Types;

namespace IYFApi.Models.Request;

public class UpdatePostRequest
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public IEnumerable<string> Tags { get; set; } = null!;
    public Status Status { get; set; }
}