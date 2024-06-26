namespace IYFApi.Models.Response;

public class PostResponse
{
    public ulong Id { get; init; }
    public string Title { get; init; } = null!;
    public string Content { get; init; } = null!;
    public IEnumerable<string> Tags { get; init; } = null!;
    public DateTime? PublishedAt { get; init; }
}

public class PostAuthorizedResponse : PostResponse
{
    public Status Status { get; init; }
    public ObjectMetadata Metadata { get; init; } = null!;
}