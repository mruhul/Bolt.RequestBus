using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IResponseHandlerAsync<TResult> : IApplicable
    {
        Task<IResponse<TResult>> Handle(IExecutionContextReader context);
        ExecutionHintType ExecutionHint { get; }
    }

    public abstract class ResponseHandlerAsync<TResult> : IResponseHandlerAsync<TResult>
    {
        public virtual ExecutionHintType ExecutionHint => ExecutionHintType.Independent;

        Task<IResponse<TResult>> IResponseHandlerAsync<TResult>.Handle(IExecutionContextReader context)
        {
            return Handle(context).ContinueWith(h => Response.Succeed(h.Result));
        }

        protected abstract Task<TResult> Handle(IExecutionContextReader context);

        public virtual bool IsApplicable(IExecutionContextReader context) => true;
    }

    public abstract class MainResponseHandlerAsync<TResult> : IResponseHandlerAsync<TResult>
    {
        public ExecutionHintType ExecutionHint => ExecutionHintType.Main;

        Task<IResponse<TResult>> IResponseHandlerAsync<TResult>.Handle(IExecutionContextReader context)
        {
            return Handle(context).ContinueWith(h => Response.Succeed(h.Result));
        }

        protected abstract Task<TResult> Handle(IExecutionContextReader context);

        public virtual bool IsApplicable(IExecutionContextReader context) => true;
    }

    public interface IResponseHandlerAsync<TRequest, TResult> : IApplicable<TRequest>
    {
        Task<IResponse<TResult>> Handle(IExecutionContextReader context, TRequest request);
        ExecutionHintType ExecutionHint { get; }
    }

    public abstract class ResponseHandlerAsync<TRequest,TResult> : IResponseHandlerAsync<TRequest, TResult>
    {
        public virtual ExecutionHintType ExecutionHint => ExecutionHintType.Independent;

        Task<IResponse<TResult>> IResponseHandlerAsync<TRequest, TResult>.Handle(IExecutionContextReader context, TRequest request)
        {
            return this.Handle(context, request)
                    .ContinueWith(t => Response.Succeed(t.Result));
        }

        protected abstract Task<TResult> Handle(IExecutionContextReader context, TRequest request);

        public virtual bool IsApplicable(IExecutionContextReader context, TRequest request) => true;
    }

    public abstract class MainResponseHandlerAsync<TRequest, TResult> : IResponseHandlerAsync<TRequest, TResult>
    {
        public ExecutionHintType ExecutionHint => ExecutionHintType.Main;

        Task<IResponse<TResult>> IResponseHandlerAsync<TRequest, TResult>.Handle(IExecutionContextReader context, TRequest request)
        {
            return this.Handle(context, request)
                    .ContinueWith(t => Response.Succeed(t.Result));
        }

        protected abstract Task<TResult> Handle(IExecutionContextReader context, TRequest request);

        public virtual bool IsApplicable(IExecutionContextReader context, TRequest request) => true;
    }
}
