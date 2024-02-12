namespace IYFApi.Models.Request;

public class CreateEventRequest
{
    public string Title { get; set; } = null!;
    public string Details { get; set; } = null!;
    
    // For events
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    // For regular events
    public string Time { get; set; } = null!;
    
    public string Location { get; set; } = null!;
}