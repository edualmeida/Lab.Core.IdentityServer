
using System.Net;
using System.Net.Mail;

namespace Lab.Core.IdentityServer.Services
{
    public class EmailNotifier(EmailOptions options) : IEmailNotifier
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var smtpClient = new SmtpClient(options.Host, options.Port);
            
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(options.Username, options.Password);
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            await smtpClient.SendMailAsync(
                new MailMessage(new MailAddress(options.FromAddress, options.FromDisplayName), new MailAddress(email))
                {
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                });
        }
    }
}
