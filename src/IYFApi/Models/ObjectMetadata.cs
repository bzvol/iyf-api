namespace IYFApi.Models;

public class ObjectMetadata
{
    public DateTime? CreatedAt { get; init; } = null;
    public string? CreatedBy { get; init; } = null;
    public DateTime? UpdatedAt { get; init; } = null;
    public string? UpdatedBy { get; init; } = null;
}