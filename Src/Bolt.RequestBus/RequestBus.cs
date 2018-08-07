using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IRequestBus
    {
        Task<IResponse> SendAsync<TRequest>(TRequest request);
        Task<IResponse<TResult>> SendAsync<TRequest,TResult>(TRequest request);
        Task PublishAsync<TRequest>(TRequest request);
    }

    public interface IResponse
    {
        bool IsSucceed { get; }
        IEnumerable<IError> Errors { get; }
    }

    public interface IResponse<T> : IResponse
    {
        T Result { get; }
    }

    public class Response : IResponse
    {
        public bool IsSucceed { get; set; }
        public IEnumerable<IError> Errors { get; set; }

        public static IResponse Succeed() => new Response { IsSucceed = true };
        public static IResponse Failed() => new Response { IsSucceed = false };

        public static IResponse<TResult> Succeed<TResult>(TResult result) => new Response<TResult> { IsSucceed = true, Result = result };
        public static IResponse<TResult> Failed<TResult>(IEnumerable<IError> errors) => new Response<TResult> { IsSucceed = false, Errors = errors ?? Enumerable.Empty<IError>() };
        public static IResponse<TResult> Failed<TResult>() => new Response<TResult> { IsSucceed = false, Errors = Enumerable.Empty<IError>() };
    }

    public class Response<T> : IResponse<T>
    {
        public bool IsSucceed { get; set; }
        public IEnumerable<IError> Errors { get; set; }
        public T Result { get; set; }
    }
    
    public interface IResponseUnit<TResult> : IResponse
    {
        TResult Result { get; }
        bool IsMainResponse { get; }
    }

    public class ResponseUnit<TResult> : IResponseUnit<TResult>
    {
        public bool IsSucceed { get; set; }
        public IEnumerable<IError> Errors { get; set; }
        public TResult Result { get; set; }
        public bool IsMainResponse { get; set; }

        public static IResponseUnit<TResult> Succeed(TResult result) => new ResponseUnit<TResult> { IsSucceed = true, Result = result };
        public static IResponseUnit<TResult> Failed(IEnumerable<IError> errors) => new ResponseUnit<TResult> { IsSucceed = false, Errors = errors ?? Enumerable.Empty<IError>() };
        public static IResponseUnit<TResult> Failed() => new ResponseUnit<TResult> { IsSucceed = false, Errors = Enumerable.Empty<IError>() };
    }

    public enum ExecutionHintType
    {
        Independent,
        Main,
        Dependent
    }
}
