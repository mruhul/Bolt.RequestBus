using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IRequestHandlerAsync<TRequest> : IApplicable<TRequest>
    {
        Task<IResponse> Handle(IExecutionContext context, TRequest request);
    }

    public abstract class RequestHandlerAsync<TRequest> : IRequestHandlerAsync<TRequest>
    {
        Task<IResponse> IRequestHandlerAsync<TRequest>.Handle(IExecutionContext context, TRequest request)
        {
            return Handle(context, request)
                .ContinueWith(t => Response.Succeed());
        }

        protected abstract Task Handle(IExecutionContext context, TRequest request);

        public virtual bool IsApplicable(IExecutionContext context, TRequest request) => true;
    }

    public interface IRequestHandlerAsync<TRequest,TResult> : IApplicable<TRequest>
    {
        Task<IResponse<TResult>> Handle(IExecutionContext context, TRequest request);
    }

    public abstract class RequestHandlerAsync<TRequest, TResult> : IRequestHandlerAsync<TRequest, TResult>
    {
        protected abstract Task<TResult> Handle(IExecutionContext context, TRequest request);

        public virtual bool IsApplicable(IExecutionContext context, TRequest request) => true;

        Task<IResponse<TResult>> IRequestHandlerAsync<TRequest, TResult>.Handle(IExecutionContext context, TRequest request)
        {
            return Handle(context, request)
                .ContinueWith(t => Response.Succeed(t.Result));
        }
    }
}
