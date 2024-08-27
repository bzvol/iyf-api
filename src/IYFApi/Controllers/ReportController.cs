using IYFApi.Filters;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IYFApi.Controllers;

[Route("api/[controller]")]
public class ReportController(IReportService reportService) : ControllerBase
{
    [HttpPost]
    [AdminAuthorizationFilter]
    public void ReportBug([FromBody] ReportBugRequest request) =>
        reportService.ReportBug(request, (UserRecordFix)HttpContext.Items["User"]!);
}