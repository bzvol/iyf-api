namespace IYFApi.Models.Request;

public class UpdateRolesRequest
{
    public bool? ContentManager { get; set; }
    public bool? GuestManager { get; set; }
    public bool? AccessManager { get; set; }
}