namespace Lab.Core.IdentityServer.Controllers.Exceptions
{
    public class ChangePasswordException : Exception
    {
        public ChangePasswordException(string message):base(message) 
        { }
    }
}
