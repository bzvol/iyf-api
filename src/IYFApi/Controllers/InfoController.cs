using IYFApi.Models.Response;
using IYFApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class InfoController(IInfoService infoService) : ControllerBase
{
    [HttpGet("counts")]
    public CountInfoResponse GetCountInfo() => infoService.GetCountInfo();
}