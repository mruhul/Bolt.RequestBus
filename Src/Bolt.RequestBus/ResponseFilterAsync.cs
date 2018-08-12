using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IResponseFilterAsync<TResult> : IApplicable
    {
        Task Filter(IExecutionContextReader context, IResponse<TResult> response);
    }

    public abstract class ResponseFilterAsync<TResult> : IResponseFilterAsync<TResult>
    {
        public abstract Task Filter(IExecutionContextReader context, IResponse<TResult> response);
        public virtual bool IsApplicable(IExecutionContextReader context) => true;
    }

    public interface IResponseCollectionFilterAsync<TResult> : IApplicable
    {
        Task Filter(IExecutionContextReader context, ICollection<IResponseUnit<TResult>> responses);
    }

    public abstract class ResponseCollectionFilterAsync<TResult> : IResponseCollectionFilterAsync<TResult>
    {
        public abstract Task Filter(IExecutionContextReader context, ICollection<IResponseUnit<TResult>> responses);

        public virtual bool IsApplicable(IExecutionContextReader context) => true;
    }

    public interface IResponseFilterAsync<TRequest,TResult> : IApplicable<TRequest>
    {
        Task Filter(IExecutionContextReader context, TRequest request, IResponse<TResult> response);
    }

    public abstract class ResponseFilterAsync<TRequest, TResult> : IResponseFilterAsync<TRequest, TResult>
    {
        public abstract Task Filter(IExecutionContextReader context, TRequest request, IResponse<TResult> response);

        public virtual bool IsApplicable(IExecutionContextReader context, TRequest request) => true;
    }

    public interface IResponseCollectionFilterAsync<TRequest,TResult> : IApplicable<TRequest>
    {
        Task Filter(IExecutionContextReader context, TRequest request, ICollection<IResponseUnit<TResult>> responses);
    }

    public abstract class ResponseCollectionFilterAsync<TRequest, TResult> : IResponseCollectionFilterAsync<TRequest, TResult>
    {
        public abstract Task Filter(IExecutionContextReader context, TRequest request, ICollection<IResponseUnit<TResult>> responses);
        
        public virtual bool IsApplicable(IExecutionContextReader context, TRequest request) => true;
    }
}
