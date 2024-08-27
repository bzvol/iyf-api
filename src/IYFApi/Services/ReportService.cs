using System.Reflection;
using IYFApi.Models;
using IYFApi.Models.Request;
using IYFApi.Services.Interfaces;
using MimeKit;

namespace IYFApi.Services;

public class ReportService(IMailService mailService) : IReportService
{
    private const string BugReportEmailTemplate = "IYFApi.Resources.BugReportEmailTemplate.html";
    private static readonly MailboxAddress Responsible = new MailboxAddress("Benjamin Zvolenszki", "ben.zvol@iyf.hu");

    public void ReportBug(ReportBugRequest request, UserRecordFix user)
    {
        var images = string.Join('\n', request.Images.Select(image =>
            $"<p><a href=\"{image}\" target=\"_blank\" rel=\"noopener noreferrer\">{image}</a></p>"));
        var template = LoadEmailTemplate(BugReportEmailTemplate, new Dictionary<string, string>
        {
            {"username", user.DisplayName},
            {"email", user.Email},
            {"message", request.Message},
            {"images", images}
        });
        mailService.SendEmail([Responsible], "Bug report", template);
    }

    private static string LoadEmailTemplate(string resourceName, Dictionary<string, string>? templateValues = null)
    {
        var assembly = Assembly.GetExecutingAssembly();
        try
        {
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);

            var template = reader.ReadToEnd();
            if (templateValues == null) return template;

            return templateValues.Aggregate(template,
                (current, pair) => current.Replace($"{{{{{pair.Key}}}}}", pair.Value));
        }
        catch (Exception e)
        {
            throw new Exception("Failed to load email template", e);
        }
    }
}