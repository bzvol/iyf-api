namespace IYFApi.Models.Request;

public class CreatePostRequest
{
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
}