using IYFApi.Models;
using IYFApi.Models.Request;

namespace IYFApi.Services.Interfaces;

public interface IReportService
{
    public void ReportBug(ReportBugRequest request, UserRecordFix user);
}