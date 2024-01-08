namespace Lab.Core.IdentityServer.Controllers.Exceptions
{
    public class UserUpdateException:Exception
    {
        public UserUpdateException(string message):base(message) 
        { }
    }
}
