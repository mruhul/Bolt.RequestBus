using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IError
    {
        string PropertyName { get; }
        string Code { get; }
        string Message { get; }
    }

    public interface IValidatorAsync<TRequest> : IApplicable<TRequest>
    {
        Task<IEnumerable<IError>> Validate(IExecutionContext context, TRequest request);
    }

    public abstract class ValidatorAsync<TRequest> : IValidatorAsync<TRequest>
    {
        public abstract Task<IEnumerable<IError>> Validate(IExecutionContext context, TRequest request);
        public virtual bool IsApplicable(IExecutionContext context, TRequest request) => true;
    }
}
