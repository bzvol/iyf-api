namespace IYFApi.Models.Response;

public class GuestResponse : EventGuest
{
    public Dictionary<string, string> Custom { get; set; } = null!;
}