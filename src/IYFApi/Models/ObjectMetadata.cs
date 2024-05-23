namespace IYFApi.Models;

public class ObjectMetadata
{
    public DateTime? CreatedAt { get; init; } = null;
    public UserRecordFix? CreatedBy { get; init; } = null;
    public DateTime? UpdatedAt { get; init; } = null;
    public UserRecordFix? UpdatedBy { get; init; } = null;
}