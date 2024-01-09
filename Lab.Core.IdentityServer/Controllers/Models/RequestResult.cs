
namespace Lab.Core.IdentityServer.Controllers.Models
{
    public class RequestResult<TResult>
    {
        public bool Succeeded { get; protected set; }
        public IEnumerable<RequestErrorDetail> Errors { get; protected set; } = new RequestErrorDetail[0];
        public TResult Result { get; protected set; }

        public RequestResult(TResult result)
        {
            Result = result;
            Succeeded = true;
        }

        public RequestResult(IEnumerable<RequestErrorDetail> identityErrors)
        {
            Succeeded = false;
            Errors = identityErrors ?? throw new ArgumentNullException(nameof(identityErrors));
        }
    }
}
