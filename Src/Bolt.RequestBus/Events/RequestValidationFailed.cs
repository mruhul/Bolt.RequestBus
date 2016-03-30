using System.Collections.Generic;

namespace Bolt.RequestBus.Events
{
    public class RequestValidationFailed<TRequest> : IEvent
        where TRequest : IRequest
    {
        public RequestValidationFailed(TRequest msg, IEnumerable<IError> errors)
        {
            Request = msg;
            Errors = errors;
        }

        public TRequest Request { get; set; }
        public IEnumerable<IError> Errors { get; set; } 
    }
}