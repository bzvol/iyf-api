using MimeKit;

namespace IYFApi.Services.Interfaces;

public interface IMailService
{
    public void SendEmail(IEnumerable<MailboxAddress> recipients, string subject, string message);
}