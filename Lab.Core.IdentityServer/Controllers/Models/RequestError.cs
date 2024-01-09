
namespace Lab.Core.IdentityServer.Controllers.Models
{
    public class RequestError
    {
        public IList<RequestErrorDetail> Errors { get;  protected set; } = new List<RequestErrorDetail>();

        public RequestError(IList<RequestErrorDetail> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }
    }
}
