using System.Collections.Generic;

namespace Bolt.RequestBus.Validators
{
    public abstract class ValidatorBase<TRequest> : IValidator<TRequest> where TRequest : IRequest
    {
        public abstract IEnumerable<IError> Validate(TRequest request);

        /// <summary>
        /// Lower order will execute first. Default value is 0
        /// </summary>
        public virtual int Order => 0;
    }
}