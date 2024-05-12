namespace IYFApi.Models.Response;

public class CountInfoResponse
{
    public PostCountInfo Posts { get; set; } = null!;
    public EventCountInfo Events { get; set; } = null!;
    public RegularEventCountInfo RegularEvents { get; set; } = null!;
}

public class PostCountInfo
{
    public int Total { get; set; }
    public int Draft { get; set; }
    public int Published { get; set; }
    public int Archived { get; set; }
}

public class EventCountInfo
{
    public int Total { get; set; }
    public int Upcoming { get; set; }
    public int Past { get; set; }
    public int Draft { get; set; }
    public int Published { get; set; }
    public int Archived { get; set; }
    public int TotalGuests { get; set; }
    public int UniqueGuests { get; set; }
}

public class RegularEventCountInfo
{
    public int Total { get; set; }
    public int Draft { get; set; }
    public int Published { get; set; }
    public int Archived { get; set; }
}
