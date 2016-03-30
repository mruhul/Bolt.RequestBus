using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bolt.RequestBus.Validators
{
    public abstract class AsyncValidatorBase<TRequest> : IAsyncValidator<TRequest> where TRequest : IRequest
    {

        public abstract Task<IEnumerable<IError>> ValidateAsync(TRequest request);

        /// <summary>
        /// Lower order will execute first. Default value is 0
        /// </summary>
        public virtual int Order => 0;
    }
}