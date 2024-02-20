using IYFApi.Exceptions;
using IYFApi.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace IYFApi.Services;

public class MailService : IMailService
{
    private const string SmtpServer = "smtp.gmail.com";
    private const int SmtpPort = 587;
    private static readonly string SmtpSenderEmail =
        Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL")
        ?? throw new ConfigurationException("SMTP_SENDER_EMAIL is not set");
    private static readonly string SmtpSenderPassword =
        Environment.GetEnvironmentVariable("SMTP_SENDER_PASSWORD")!
        ?? throw new ConfigurationException("SMTP_SENDER_PASSWORD is not set");
    private const string SmtpSenderName = "IYF Hungary";

    public void SendEmail(IEnumerable<MailboxAddress> recipients, string subject, string body)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(SmtpSenderName, SmtpSenderEmail));
        mime.To.AddRange(recipients);
        mime.Subject = subject;
        mime.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        client.Connect(SmtpServer, SmtpPort, false);
        client.Authenticate(SmtpSenderEmail, SmtpSenderPassword);
        client.Send(mime);
        client.Disconnect(true);
    }
}