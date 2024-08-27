namespace IYFApi.Models.Request;

public class ReportBugRequest
{
    public string Message { get; set; } = null!;
    public string[] Images { get; set; } = [];
}