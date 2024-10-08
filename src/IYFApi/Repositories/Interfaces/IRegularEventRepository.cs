﻿using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Models.Response;

namespace IYFApi.Repositories.Interfaces;

public interface IRegularEventRepository
{
    public IEnumerable<RegularEventResponse> GetAllEvents();
    public Task<IEnumerable<RegularEventAuthorizedResponse>> GetAllEventsAuthorized();
    public RegularEventResponse GetEvent(ulong id);
    public Task<RegularEventAuthorizedResponse> GetEventAuthorized(ulong id);
    public Task<RegularEventResponse> CreateEvent(CreateRegularEventRequest value, string userId);
    public Task<RegularEventResponse> UpdateEvent(ulong id, UpdateRegularEventRequest value, string userId);
    public Task<RegularEventResponse> DeleteEvent(ulong id);
}