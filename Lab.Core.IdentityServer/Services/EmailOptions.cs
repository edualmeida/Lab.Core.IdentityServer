using System.Security.Cryptography.X509Certificates;

namespace Lab.Core.IdentityServer.Services
{
    public class EmailOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromAddress { get; set; }
        public string FromDisplayName { get; set; }

        public EmailOptions(string fromAddress, string fromDisplayName)
        {
            FromAddress = fromAddress;
            FromDisplayName = fromDisplayName;
        }
    }
}
