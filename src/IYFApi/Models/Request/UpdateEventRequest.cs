﻿using IYFApi.Models.Response;

namespace IYFApi.Models.Request;

public class UpdateEventRequest
{
    public string Title { get; init; } = null!;
    public string Details { get; init; } = null!;
    public EventSchedule Schedule { get; init; } = null!;
    public Status Status { get; init; }
}