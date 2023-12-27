namespace Lab.Core.IdentityServer.Services
{
    public interface IEmailNotifier
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
