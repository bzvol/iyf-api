using IYFApi.Models.Response;

namespace IYFApi.Services.Interfaces;

public interface IInfoService
{
    public CountInfoResponse GetCountInfo();
}