namespace IYFApi.Models.Response;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; } = null;
    public string? StackTrace { get; set; } = null;
}