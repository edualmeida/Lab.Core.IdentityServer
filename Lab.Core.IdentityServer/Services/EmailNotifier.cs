
namespace Lab.Core.IdentityServer.Services
{
    public class EmailNotifier : IEmailNotifier
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}
