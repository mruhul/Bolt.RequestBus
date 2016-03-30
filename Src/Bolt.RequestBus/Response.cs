using System.Collections.Generic;
using System.Linq;

namespace Bolt.RequestBus
{
    public class Response : IResponse
    {
        private static readonly IResponse SucceedInstance = new Response(true);
        private static readonly IResponse FailedInstance = new Response(false);

        public static IResponse Succeed()
        {
            return SucceedInstance;
        }

        public static IResponse Failed()
        {
            return FailedInstance;
        }

        public static IResponse Failed(IEnumerable<IError> errors)
        {
            return new Response(errors);
        }

        public static IResponse<T> Succeed<T>(T value)
        {
            return new Response<T>(value);
        }
        public static IResponse<T> Failed<T>(IEnumerable<IError> errors)
        {
            return new Response<T>(errors ?? Enumerable.Empty<IError>());
        }

        private Response(bool succeed)
        {
            IsSucceed = succeed;
        }

        private Response(IEnumerable<IError> errors)
        {
            IsSucceed = false;
            Errors = errors;
        }

        private bool IsSucceed { get; }
        bool IResponse.Succeed => IsSucceed;

        public IEnumerable<IError> Errors { get; private set; }
    }

    internal class Response<T> : IResponse<T>
    {
        internal Response(T value)
        {
            IsSucceed = true;
            Value = value;
        }

        internal Response(IEnumerable<IError> errors)
        {
            IsSucceed = false;
            Errors = errors;
        }

        public IEnumerable<IError> Errors { get; set; }

        private bool IsSucceed { get; }
        bool IResponse.Succeed => IsSucceed;

        public T Value { get; set; }
    }
}
