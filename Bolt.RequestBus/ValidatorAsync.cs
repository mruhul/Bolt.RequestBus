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

    public class Error : IError
    {
        public string PropertyName { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public static IError Create(string message) => new Error { Message = message };
        public static IError Create(string propertyName, string message) => new Error { Message = message, PropertyName = propertyName };
        public static IError Create(string propertyName, string message, string code) => new Error { Message = message, PropertyName = propertyName, Code = code };
    }

    public interface IValidatorAsync<TRequest> : IApplicable<TRequest>
    {
        Task<IEnumerable<IError>> Validate(IExecutionContextReader context, TRequest request);
    }

    public abstract class ValidatorAsync<TRequest> : IValidatorAsync<TRequest>
    {
        public abstract Task<IEnumerable<IError>> Validate(IExecutionContextReader context, TRequest request);
        public virtual bool IsApplicable(IExecutionContextReader context, TRequest request) => true;
    }
}
