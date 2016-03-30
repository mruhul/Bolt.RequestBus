using System.Collections.Generic;

namespace Bolt.RequestBus
{
    public interface IValidator<in TRequest> where TRequest : IRequest
    {
        IEnumerable<IError> Validate(TRequest request);
        /// <summary>
        /// Lower order value will execute first
        /// </summary>
        int Order { get; }
    }
}