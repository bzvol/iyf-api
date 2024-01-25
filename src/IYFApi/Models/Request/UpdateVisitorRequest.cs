namespace IYFApi.Models.Request;

public class UpdateVisitorRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int? Age { get; set; }
    public string? City { get; set; }
    public string? Source { get; set; }
}