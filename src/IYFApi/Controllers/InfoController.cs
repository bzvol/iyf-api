using IYFApi.Filters;
using IYFApi.Models.Response;
using IYFApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class InfoController(IInfoService infoService) : ControllerBase
{
    [HttpGet("counts")]
    [AdminAuthorizationFilter]
    public CountInfoResponse GetCountInfo() => infoService.GetCountInfo();
}