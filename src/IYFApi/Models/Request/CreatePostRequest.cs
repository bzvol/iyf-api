namespace IYFApi.Models.Request;

public class CreatePostRequest
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
}